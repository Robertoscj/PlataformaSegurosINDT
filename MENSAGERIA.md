# 📨 Guia de Mensageria AWS (SNS + SQS)

Este guia explica a implementação de mensageria assíncrona usando **AWS SNS** (Simple Notification Service) e **AWS SQS** (Simple Queue Service) para comunicação entre microserviços.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Configuração](#configuração)
- [Como Funciona](#como-funciona)
- [Desenvolvimento Local](#desenvolvimento-local)
- [Produção AWS](#produção-aws)
- [Testando](#testando)

---

## 🎯 Visão Geral

A mensageria implementada permite **comunicação assíncrona e desacoplada** entre os microserviços:

- **PropostaService** publica eventos quando uma proposta é aprovada
- **ContratacaoService** consome esses eventos automaticamente
- Usa padrão **Pub/Sub** (Publisher/Subscriber)
- Implementa **Arquitetura Hexagonal** (Portas e Adaptadores)

### ✅ Benefícios

- **Desacoplamento**: Serviços não dependem de chamadas HTTP síncronas
- **Resiliência**: Mensagens são persistidas e processadas com retry automático
- **Escalabilidade**: Processamento assíncrono permite melhor throughput
- **Confiabilidade**: Garantia de entrega com Dead Letter Queues (DLQ)

---

## 🏗️ Arquitetura

```
┌─────────────────────┐
│  PropostaService    │
│                     │
│  [Proposta Aprovada]│
│         │           │
│         ▼           │
│   IMessagePublisher │ ◄─── Porta (Interface)
│         │           │
│         ▼           │
│  SnsMessagePublisher│ ◄─── Adaptador AWS SNS
└──────────┬──────────┘
           │
           │ publica evento
           ▼
    ┌─────────────┐
    │   AWS SNS   │
    │   (Topic)   │
    └──────┬──────┘
           │
           │ encaminha
           ▼
    ┌─────────────┐
    │   AWS SQS   │
    │   (Queue)   │
    └──────┬──────┘
           │
           │ consome evento
           ▼
┌──────────────────────┐
│  ContratacaoService  │
│                      │
│   IMessageConsumer   │ ◄─── Porta (Interface)
│          │           │
│          ▼           │
│  SqsMessageConsumer  │ ◄─── Adaptador AWS SQS
│          │           │
│          ▼           │
│  [Processa Evento]   │
└──────────────────────┘
```

### Recursos AWS Criados

| Recurso | Nome | Descrição |
|---------|------|-----------|
| **SNS Topic** | `proposta-aprovada` | Tópico para publicar eventos de proposta aprovada |
| **SQS Queue** | `proposta-aprovada-queue` | Fila que recebe eventos do tópico |
| **SNS Subscription** | Topic → Queue | Inscrição da fila no tópico |

---

## ⚙️ Configuração

### appsettings.json

#### PropostaService

```json
{
  "AWS": {
    "Profile": "default",
    "Region": "us-east-1",
    "SNS": {
      "PropostaAprovadaTopic": "arn:aws:sns:us-east-1:ACCOUNT_ID:proposta-aprovada"
    }
  }
}
```

#### ContratacaoService

```json
{
  "AWS": {
    "Profile": "default",
    "Region": "us-east-1",
    "SQS": {
      "PropostaAprovadaQueue": "https://sqs.us-east-1.amazonaws.com/ACCOUNT_ID/proposta-aprovada-queue"
    }
  }
}
```

---

## 🔄 Como Funciona

### 1. Publicação de Evento (PropostaService)

Quando uma proposta é **aprovada**, o evento é publicado automaticamente:

```csharp
// Use Case: AlterarStatusPropostaUseCase.cs
if (novoStatus == StatusProposta.Aprovada)
{
    var evento = new PropostaAprovadaEvent
    {
        PropostaId = proposta.Id,
        NomeCliente = proposta.NomeCliente,
        CpfCliente = proposta.CpfCliente.Numero,
        TipoSeguro = proposta.TipoSeguro,
        ValorCobertura = proposta.ValorCobertura.Valor,
        ValorPremio = proposta.ValorPremio.Valor,
        DataAprovacao = DateTime.UtcNow
    };

    await _messagePublisher.PublishAsync(evento, topicArn);
}
```

### 2. Consumo de Evento (ContratacaoService)

O **Background Service** consome eventos automaticamente:

```csharp
// BackgroundService: PropostaAprovadaConsumerService.cs
await _messageConsumer.StartConsumingAsync<PropostaAprovadaEvent>(
    queueUrl,
    ProcessarPropostaAprovadaAsync
);

private async Task ProcessarPropostaAprovadaAsync(PropostaAprovadaEvent evento)
{
    _logger.LogInformation(
        "Proposta aprovada recebida: {PropostaId} - Cliente: {NomeCliente}",
        evento.PropostaId,
        evento.NomeCliente
    );
    
    // Processar evento...
}
```

### 3. Fluxo Completo

```
1. User → PATCH /api/propostas/{id}/status (Status = Aprovada)
2. PropostaService → Atualiza banco de dados
3. PropostaService → Publica evento no SNS
4. SNS → Encaminha para SQS
5. ContratacaoService → Consome da fila SQS
6. ContratacaoService → Processa evento
7. SQS → Mensagem deletada após sucesso
```

---

## 🧪 Desenvolvimento Local

Para desenvolvimento, usamos **LocalStack** para simular AWS localmente.

### 1. Iniciar LocalStack

```bash
# Via Docker Compose
docker-compose up -d localstack

# Verificar se está rodando
curl http://localhost:4566/_localstack/health
```

### 2. Criar Recursos AWS

```bash
# Executar script de inicialização
./scripts/init-localstack.sh

# Verificar recursos criados
awslocal sns list-topics
awslocal sqs list-queues
```

### 3. Executar Serviços

```bash
# Terminal 1: PropostaService
cd src/PropostaService/PropostaService.API
dotnet run

# Terminal 2: ContratacaoService  
cd src/ContratacaoService/ContratacaoService.API
dotnet run
```

### 4. Configuração Local

Os serviços usam `appsettings.Development.json` com LocalStack:

```json
{
  "AWS": {
    "ServiceURL": "http://localhost:4566",
    "Region": "us-east-1"
  }
}
```

---

## ☁️ Produção AWS

### 1. Criar Recursos na AWS

```bash
# Criar tópico SNS
aws sns create-topic --name proposta-aprovada --region us-east-1

# Criar fila SQS
aws sqs create-queue --queue-name proposta-aprovada-queue --region us-east-1

# Obter ARNs
TOPIC_ARN=$(aws sns list-topics --query 'Topics[?contains(TopicArn, `proposta-aprovada`)].TopicArn' --output text)
QUEUE_URL=$(aws sqs get-queue-url --queue-name proposta-aprovada-queue --query 'QueueUrl' --output text)
QUEUE_ARN=$(aws sqs get-queue-attributes --queue-url $QUEUE_URL --attribute-names QueueArn --query 'Attributes.QueueArn' --output text)

# Inscrever fila no tópico
aws sns subscribe --topic-arn $TOPIC_ARN --protocol sqs --notification-endpoint $QUEUE_ARN
```

### 2. Configurar IAM

As aplicações precisam de permissões:

#### PropostaService (SNS)

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "sns:Publish"
      ],
      "Resource": "arn:aws:sns:us-east-1:*:proposta-aprovada"
    }
  ]
}
```

#### ContratacaoService (SQS)

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "sqs:ReceiveMessage",
        "sqs:DeleteMessage",
        "sqs:GetQueueAttributes"
      ],
      "Resource": "arn:aws:sqs:us-east-1:*:proposta-aprovada-queue"
    }
  ]
}
```

### 3. Variáveis de Ambiente (Produção)

```bash
# PropostaService
AWS__Region=us-east-1
AWS__SNS__PropostaAprovadaTopic=arn:aws:sns:us-east-1:123456789:proposta-aprovada

# ContratacaoService
AWS__Region=us-east-1
AWS__SQS__PropostaAprovadaQueue=https://sqs.us-east-1.amazonaws.com/123456789/proposta-aprovada-queue
```

---

## 🧪 Testando

### 1. Aprovar uma Proposta

```bash
# Criar proposta
curl -X POST http://localhost:5001/api/propostas \
  -H "Content-Type: application/json" \
  -d '{
    "nomeCliente": "João Silva",
    "cpfCliente": "123.456.789-00",
    "tipoSeguro": "Vida",
    "valorCobertura": 100000,
    "valorPremio": 500
  }'

# Guardar o ID retornado
PROPOSTA_ID="..."

# Aprovar proposta (isso publica o evento!)
curl -X PATCH http://localhost:5001/api/propostas/$PROPOSTA_ID/status \
  -H "Content-Type: application/json" \
  -d '{"novoStatus": 2}'
```

### 2. Verificar Logs

#### PropostaService
```
✅ Evento PropostaAprovada publicado com sucesso. PropostaId: {id}
📢 Mensagem publicada no SNS. MessageId: xxx
```

#### ContratacaoService
```
📬 Recebido evento PropostaAprovada. PropostaId: {id}
✅ Evento processado com sucesso
```

### 3. Verificar Fila (LocalStack)

```bash
# Ver mensagens na fila
awslocal sqs receive-message \
  --queue-url http://localhost:4566/000000000000/proposta-aprovada-queue

# Ver atributos da fila
awslocal sqs get-queue-attributes \
  --queue-url http://localhost:4566/000000000000/proposta-aprovada-queue \
  --attribute-names All
```

---

## 🏗️ Implementação (Arquitetura Hexagonal)

### Portas (Interfaces)

```csharp
// Domain/Ports/IMessagePublisher.cs
public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string topic, CancellationToken ct = default) where T : class;
}

// Domain/Ports/IMessageConsumer.cs
public interface IMessageConsumer
{
    Task StartConsumingAsync<T>(string queueUrl, Func<T, Task> handler, CancellationToken ct = default) where T : class;
    Task StopConsumingAsync();
}
```

### Adaptadores (Implementações)

```csharp
// Infrastructure/Messaging/SnsMessagePublisher.cs
public class SnsMessagePublisher : IMessagePublisher
{
    private readonly IAmazonSimpleNotificationService _snsClient;
    
    public async Task PublishAsync<T>(T message, string topicArn, CancellationToken ct)
    {
        var messageJson = JsonSerializer.Serialize(message);
        await _snsClient.PublishAsync(new PublishRequest
        {
            TopicArn = topicArn,
            Message = messageJson
        }, ct);
    }
}

// Infrastructure/Messaging/SqsMessageConsumer.cs
public class SqsMessageConsumer : IMessageConsumer
{
    private readonly IAmazonSQS _sqsClient;
    
    public async Task StartConsumingAsync<T>(string queueUrl, Func<T, Task> handler, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var response = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20 // Long polling
            }, ct);

            foreach (var message in response.Messages)
            {
                var deserializedMessage = JsonSerializer.Deserialize<T>(message.Body);
                await handler(deserializedMessage);
                await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle, ct);
            }
        }
    }
}
```

---

## 📊 Monitoramento

### Métricas Importantes

- **SNS**: Número de mensagens publicadas, falhas
- **SQS**: Mensagens na fila, mensagens processadas, DLQ
- **Application**: Tempo de processamento, erros

### CloudWatch (Produção)

```bash
# Ver métricas SNS
aws cloudwatch get-metric-statistics \
  --namespace AWS/SNS \
  --metric-name NumberOfMessagesPublished \
  --dimensions Name=TopicName,Value=proposta-aprovada

# Ver métricas SQS
aws cloudwatch get-metric-statistics \
  --namespace AWS/SQS \
  --metric-name NumberOfMessagesReceived \
  --dimensions Name=QueueName,Value=proposta-aprovada-queue
```

---

## 🚨 Troubleshooting

### Problema: Eventos não estão sendo publicados

**Verificar:**
1. LocalStack está rodando? `curl http://localhost:4566/_localstack/health`
2. Tópico SNS existe? `awslocal sns list-topics`
3. Configuração correta? Verificar `appsettings.json`
4. Logs do PropostaService: mensagens de erro?

### Problema: Eventos não estão sendo consumidos

**Verificar:**
1. Fila SQS existe? `awslocal sqs list-queues`
2. Inscrição SNS→SQS existe? `awslocal sns list-subscriptions`
3. Background Service iniciou? Verificar logs do ContratacaoService
4. Mensagens na fila? `awslocal sqs receive-message --queue-url ...`

### Problema: Mensagens ficam presas na fila

**Possíveis causas:**
- Erro no processamento (exception não tratada)
- Timeout de visibilidade muito curto
- Dead Letter Queue configurada incorretamente

**Solução:**
```bash
# Purgar fila (desenvolvimento)
awslocal sqs purge-queue --queue-url http://localhost:4566/000000000000/proposta-aprovada-queue
```

---

## 📚 Recursos Adicionais

- [AWS SNS Documentation](https://docs.aws.amazon.com/sns/)
- [AWS SQS Documentation](https://docs.aws.amazon.com/sqs/)
- [LocalStack Documentation](https://docs.localstack.cloud/)
- [AWSSDK.NET](https://aws.amazon.com/sdk-for-net/)

---

## ✅ Checklist de Implementação

- [x] Pacotes AWS SDK instalados
- [x] Interfaces (Portas) criadas
- [x] Adaptadores AWS implementados
- [x] Eventos de domínio definidos
- [x] Publicação configurada no PropostaService
- [x] Consumo configurado no ContratacaoService
- [x] LocalStack integrado no Docker Compose
- [x] Scripts de inicialização criados
- [x] Configurações nos appsettings
- [x] Documentação completa

---

**Implementação completa de mensageria assíncrona! 🚀**


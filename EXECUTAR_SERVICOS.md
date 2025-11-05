# 🚀 Como Executar os Serviços

Este guia rápido mostra como executar os serviços da plataforma de seguros.

## 📋 Pré-requisitos

- .NET 8.0 SDK instalado
- PostgreSQL rodando (local ou Docker)

## 🎯 Execução Rápida

### ✅ Opção 1: Com Hot Reload (Recomendado para Desenvolvimento)

O hot reload detecta alterações no código e reinicia automaticamente o serviço.

#### PropostaService (Terminal 1)
```bash
cd src/PropostaService/PropostaService.API
dotnet watch run
```

#### ContratacaoService (Terminal 2)
```bash
cd src/ContratacaoService/ContratacaoService.API
dotnet watch run
```

### ✅ Opção 2: Execução Normal

```bash
# PropostaService (Terminal 1)
cd src/PropostaService/PropostaService.API
dotnet run

# ContratacaoService (Terminal 2)
cd src/ContratacaoService/ContratacaoService.API
dotnet run
```

### ✅ Opção 3: Usando Scripts Helper

```bash
# PropostaService com hot reload
./scripts/executar-servico.sh proposta watch

# ContratacaoService com hot reload
./scripts/executar-servico.sh contratacao watch

# Execução normal (sem hot reload)
./scripts/executar-servico.sh proposta run
./scripts/executar-servico.sh contratacao run
```

## 🌐 URLs dos Serviços

Após iniciar os serviços, acesse:

### PropostaService
- **Swagger UI**: http://localhost:5001
- **Health Check**: http://localhost:5001/health
- **API Base**: http://localhost:5001/api/propostas

### ContratacaoService
- **Swagger UI**: http://localhost:5002
- **Health Check**: http://localhost:5002/health
- **API Base**: http://localhost:5002/api/contratacoes

## 🎨 Swagger na Raiz

Os serviços estão configurados para exibir o Swagger diretamente na raiz:

- Acesse `http://localhost:5001` → Swagger do PropostaService
- Acesse `http://localhost:5002` → Swagger do ContratacaoService

**Não é necessário** adicionar `/swagger` na URL!

## 🔄 Diferenças entre `dotnet run` e `dotnet watch run`

### `dotnet run`
- ✅ Execução normal
- ✅ Mais rápido para iniciar
- ❌ Requer reiniciar manualmente ao modificar código

### `dotnet watch run` (Hot Reload)
- ✅ Detecta mudanças automaticamente
- ✅ Reinicia o serviço ao salvar arquivos
- ✅ Ideal para desenvolvimento
- ❌ Um pouco mais lento para iniciar
- ⚠️ Usa mais recursos (monitora arquivos)

## 🗄️ Banco de Dados

### Usando Docker (Recomendado)
```bash
docker run --name postgres-local \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:16
```

### Verificar se está rodando
```bash
docker ps | grep postgres
```

## 🔧 Configuração

As configurações de banco de dados estão em:
- `src/PropostaService/PropostaService.API/appsettings.json`
- `src/ContratacaoService/ContratacaoService.API/appsettings.json`

Connection string padrão:
```
Host=localhost;Port=5432;Database={nome_db};Username=postgres;Password=postgres
```

## ⚡ Migrations Automáticas

Os serviços aplicam as migrations automaticamente na primeira execução em ambiente de desenvolvimento. Você verá mensagens como:

```
Applying migration 'InitialCreate'...
Done.
```

## 🐛 Solução de Problemas

### ❌ Erro: "Address already in use"
Outra aplicação está usando a porta 5001 ou 5002.

**Solução**: Pare o processo ou altere a porta em `Properties/launchSettings.json`

### ❌ Erro: "Unable to connect to database"
O PostgreSQL não está rodando.

**Solução**: 
```bash
# Inicie o PostgreSQL
docker start postgres-local

# Ou crie um novo container
docker run --name postgres-local \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:16
```

### ❌ Erro: "Package restore failed"
Os pacotes NuGet não foram restaurados corretamente.

**Solução**:
```bash
# Limpar cache
dotnet nuget locals all --clear

# Restaurar novamente
dotnet restore --no-cache
```

### ❌ Swagger não abre automaticamente
O navegador não foi aberto automaticamente.

**Solução**: Abra manualmente:
- http://localhost:5001 (PropostaService)
- http://localhost:5002 (ContratacaoService)

## 💡 Dicas

1. **Use hot reload durante desenvolvimento**: `dotnet watch run` economiza tempo
2. **Swagger está na raiz**: Acesse diretamente `http://localhost:500X`
3. **Health checks**: Use `/health` para verificar o status dos serviços
4. **Logs detalhados**: Em desenvolvimento, os logs são mais verbosos
5. **Migrations automáticas**: Apenas em ambiente Development

## 🎯 Testando os Serviços

### 1. Verificar Health
```bash
curl http://localhost:5001/health
curl http://localhost:5002/health
```

### 2. Criar uma Proposta
```bash
curl -X POST http://localhost:5001/api/propostas \
  -H "Content-Type: application/json" \
  -d '{
    "nomeCliente": "João da Silva",
    "cpfCliente": "123.456.789-09",
    "tipoSeguro": "Vida",
    "valorCobertura": 100000.00,
    "valorPremio": 500.00
  }'
```

### 3. Listar Propostas
```bash
curl http://localhost:5001/api/propostas
```

## 📚 Documentação Completa

Para mais informações, consulte:
- [QUICK_START.md](QUICK_START.md) - Guia de início rápido completo
- [README.md](README.md) - Documentação completa do projeto
- [ARQUITETURA.md](ARQUITETURA.md) - Detalhes da arquitetura

---

**Boa codificação! 🚀**


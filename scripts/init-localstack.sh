#!/bin/bash

# Script para inicializar recursos AWS no LocalStack
# Este script cria os tópicos SNS e filas SQS necessários

set -e

echo "🚀 Inicializando recursos AWS no LocalStack..."

# Aguardar LocalStack estar pronto
echo "⏳ Aguardando LocalStack inicializar..."
sleep 5

# Configurar AWS CLI para usar LocalStack
export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test
export AWS_DEFAULT_REGION=us-east-1

ENDPOINT_URL="http://localhost:4566"

# Criar tópico SNS
echo "📢 Criando tópico SNS: proposta-aprovada..."
TOPIC_ARN=$(awslocal sns create-topic \
    --name proposta-aprovada \
    --output text \
    --query 'TopicArn' || echo "")

if [ -z "$TOPIC_ARN" ]; then
    echo "⚠️  Tópico já existe ou erro ao criar"
    TOPIC_ARN="arn:aws:sns:us-east-1:000000000000:proposta-aprovada"
else
    echo "✅ Tópico SNS criado: $TOPIC_ARN"
fi

# Criar fila SQS
echo "📬 Criando fila SQS: proposta-aprovada-queue..."
QUEUE_URL=$(awslocal sqs create-queue \
    --queue-name proposta-aprovada-queue \
    --output text \
    --query 'QueueUrl' || echo "")

if [ -z "$QUEUE_URL" ]; then
    echo "⚠️  Fila já existe ou erro ao criar"
    QUEUE_URL="http://localhost:4566/000000000000/proposta-aprovada-queue"
else
    echo "✅ Fila SQS criada: $QUEUE_URL"
fi

# Obter ARN da fila
QUEUE_ARN=$(awslocal sqs get-queue-attributes \
    --queue-url "$QUEUE_URL" \
    --attribute-names QueueArn \
    --output text \
    --query 'Attributes.QueueArn')

echo "📋 Queue ARN: $QUEUE_ARN"

# Inscrever fila no tópico SNS
echo "🔗 Inscrevendo fila no tópico SNS..."
awslocal sns subscribe \
    --topic-arn "$TOPIC_ARN" \
    --protocol sqs \
    --notification-endpoint "$QUEUE_ARN" || echo "⚠️  Subscription já existe"

echo ""
echo "✅ Recursos AWS inicializados com sucesso!"
echo ""
echo "📋 Recursos criados:"
echo "   - SNS Topic: $TOPIC_ARN"
echo "   - SQS Queue: $QUEUE_URL"
echo ""
echo "🔍 Para verificar os recursos:"
echo "   awslocal sns list-topics"
echo "   awslocal sqs list-queues"
echo ""


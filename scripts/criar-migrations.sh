#!/bin/bash

# Script para criar migrations do Entity Framework Core

echo "=========================================="
echo "Criando Migrations - Plataforma de Seguros"
echo "=========================================="

# Navegar para o diretório do projeto
cd "$(dirname "$0")/.."

# Migration para PropostaService
echo ""
echo "1. Criando migration para PropostaService..."
cd src/PropostaService/PropostaService.Infrastructure
dotnet ef migrations add InitialCreate \
    --startup-project ../PropostaService.API/PropostaService.API.csproj \
    --context PropostaDbContext \
    --output-dir Migrations

if [ $? -eq 0 ]; then
    echo "✓ Migration do PropostaService criada com sucesso!"
else
    echo "✗ Erro ao criar migration do PropostaService"
    exit 1
fi

# Voltar ao diretório raiz
cd ../../..

# Migration para ContratacaoService
echo ""
echo "2. Criando migration para ContratacaoService..."
cd src/ContratacaoService/ContratacaoService.Infrastructure
dotnet ef migrations add InitialCreate \
    --startup-project ../ContratacaoService.API/ContratacaoService.API.csproj \
    --context ContratacaoDbContext \
    --output-dir Migrations

if [ $? -eq 0 ]; then
    echo "✓ Migration do ContratacaoService criada com sucesso!"
else
    echo "✗ Erro ao criar migration do ContratacaoService"
    exit 1
fi

echo ""
echo "=========================================="
echo "Migrations criadas com sucesso!"
echo "=========================================="
echo ""
echo "Para aplicar as migrations, execute:"
echo "  ./scripts/aplicar-migrations.sh"
echo ""


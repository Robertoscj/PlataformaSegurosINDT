#!/bin/bash

# Script para aplicar migrations do Entity Framework Core

echo "=========================================="
echo "Aplicando Migrations - Plataforma de Seguros"
echo "=========================================="

# Navegar para o diretório do projeto
cd "$(dirname "$0")/.."

# Aplicar migration para PropostaService
echo ""
echo "1. Aplicando migration para PropostaService..."
cd src/PropostaService/PropostaService.Infrastructure
dotnet ef database update \
    --startup-project ../PropostaService.API/PropostaService.API.csproj \
    --context PropostaDbContext

if [ $? -eq 0 ]; then
    echo "✓ Migration do PropostaService aplicada com sucesso!"
else
    echo "✗ Erro ao aplicar migration do PropostaService"
    exit 1
fi

# Voltar ao diretório raiz
cd ../../..

# Aplicar migration para ContratacaoService
echo ""
echo "2. Aplicando migration para ContratacaoService..."
cd src/ContratacaoService/ContratacaoService.Infrastructure
dotnet ef database update \
    --startup-project ../ContratacaoService.API/ContratacaoService.API.csproj \
    --context ContratacaoDbContext

if [ $? -eq 0 ]; then
    echo "✓ Migration do ContratacaoService aplicada com sucesso!"
else
    echo "✗ Erro ao aplicar migration do ContratacaoService"
    exit 1
fi

echo ""
echo "=========================================="
echo "Migrations aplicadas com sucesso!"
echo "=========================================="
echo ""


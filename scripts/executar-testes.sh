#!/bin/bash

# Script para executar todos os testes do projeto

echo "=========================================="
echo "Executando Testes - Plataforma de Seguros"
echo "=========================================="

# Navegar para o diretório do projeto
cd "$(dirname "$0")/.."

echo ""
echo "Restaurando dependências..."
dotnet restore

echo ""
echo "=========================================="
echo "1. Testes PropostaService"
echo "=========================================="
dotnet test tests/PropostaService.Tests/PropostaService.Tests.csproj \
    --configuration Release \
    --logger "console;verbosity=detailed" \
    --collect:"XPlat Code Coverage"

if [ $? -ne 0 ]; then
    echo "❌ Falha nos testes do PropostaService"
    exit 1
fi

echo ""
echo "=========================================="
echo "2. Testes ContratacaoService"
echo "=========================================="
dotnet test tests/ContratacaoService.Tests/ContratacaoService.Tests.csproj \
    --configuration Release \
    --logger "console;verbosity=detailed" \
    --collect:"XPlat Code Coverage"

if [ $? -ne 0 ]; then
    echo "❌ Falha nos testes do ContratacaoService"
    exit 1
fi

echo ""
echo "=========================================="
echo "✅ Todos os testes passaram com sucesso!"
echo "=========================================="
echo ""


#!/bin/bash

# Script para fazer build completo da solução

echo "=========================================="
echo "Build - Plataforma de Seguros"
echo "=========================================="

# Navegar para o diretório do projeto
cd "$(dirname "$0")/.."

echo ""
echo "Limpando solução..."
dotnet clean

echo ""
echo "Restaurando dependências..."
dotnet restore

echo ""
echo "Compilando solução..."
dotnet build --configuration Release

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "✅ Build concluído com sucesso!"
    echo "=========================================="
    echo ""
else
    echo ""
    echo "=========================================="
    echo "❌ Erro no build"
    echo "=========================================="
    echo ""
    exit 1
fi


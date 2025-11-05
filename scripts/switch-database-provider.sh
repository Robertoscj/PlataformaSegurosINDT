#!/bin/bash

# Script para facilitar a troca de provedor de banco de dados

echo "=========================================="
echo "Trocar Provedor de Banco de Dados"
echo "Plataforma de Seguros"
echo "=========================================="

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Verificar se estamos no diretório correto
if [ ! -f "PlataformaSeguros.sln" ]; then
    echo -e "${RED}❌ Erro: Execute este script no diretório raiz do projeto${NC}"
    exit 1
fi

echo ""
echo "Provedores disponíveis:"
echo "1) PostgreSQL (Padrão)"
echo "2) SQL Server"
echo "3) MySQL / MariaDB"
echo "4) InMemory (Testes)"
echo ""
read -p "Escolha o provedor [1-4]: " choice

case $choice in
    1)
        PROVIDER="PostgreSQL"
        CONNECTION_STRING_EXAMPLE="Host=localhost;Port=5432;Database=propostadb;Username=postgres;Password=postgres"
        PACKAGE="Npgsql.EntityFrameworkCore.PostgreSQL"
        INSTALLED=true
        ;;
    2)
        PROVIDER="SqlServer"
        CONNECTION_STRING_EXAMPLE="Server=localhost;Database=propostadb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
        PACKAGE="Microsoft.EntityFrameworkCore.SqlServer"
        INSTALLED=true
        ;;
    3)
        PROVIDER="MySql"
        CONNECTION_STRING_EXAMPLE="Server=localhost;Database=propostadb;User=root;Password=password"
        PACKAGE="Pomelo.EntityFrameworkCore.MySql"
        INSTALLED=true
        ;;
    4)
        PROVIDER="InMemory"
        CONNECTION_STRING_EXAMPLE="TestDatabase"
        PACKAGE="Microsoft.EntityFrameworkCore.InMemory"
        INSTALLED=true
        ;;
    *)
        echo -e "${RED}❌ Opção inválida${NC}"
        exit 1
        ;;
esac

echo ""
echo -e "${BLUE}Provedor selecionado: $PROVIDER${NC}"
echo ""

# Verificar se o pacote está instalado
if [ "$INSTALLED" = false ]; then
    echo -e "${YELLOW}⚠️  O pacote $PACKAGE não está instalado por padrão.${NC}"
    echo ""
    read -p "Deseja instalar agora? (s/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Ss]$ ]]; then
        echo ""
        echo "Instalando $PACKAGE..."

        cd src/PropostaService/PropostaService.Infrastructure
        dotnet add package "$PACKAGE"

        cd ../../ContratacaoService/ContratacaoService.Infrastructure
        dotnet add package "$PACKAGE"

        cd ../../..

        echo -e "${GREEN}✓${NC} Pacote instalado!"
    else
        echo -e "${YELLOW}⚠️  Você precisará instalar manualmente:${NC}"
        echo "  dotnet add package $PACKAGE"
        echo ""
    fi
fi

# Solicitar connection string
echo ""
echo "Connection String exemplo para $PROVIDER:"
echo -e "${BLUE}$CONNECTION_STRING_EXAMPLE${NC}"
echo ""
read -p "Digite a connection string (Enter para usar padrão): " user_conn_string

if [ -z "$user_conn_string" ]; then
    CONNECTION_STRING="$CONNECTION_STRING_EXAMPLE"
else
    CONNECTION_STRING="$user_conn_string"
fi

# Atualizar appsettings.json
echo ""
echo "Atualizando configurações..."

# PropostaService
PROPOSTA_APPSETTINGS="src/PropostaService/PropostaService.API/appsettings.Development.json"
if [ -f "$PROPOSTA_APPSETTINGS" ]; then
    # Backup
    cp "$PROPOSTA_APPSETTINGS" "$PROPOSTA_APPSETTINGS.backup"

    # Usar jq se disponível, caso contrário, manual
    if command -v jq &> /dev/null; then
        jq ".Database.Provider = \"$PROVIDER\" | .ConnectionStrings.DefaultConnection = \"$CONNECTION_STRING\"" "$PROPOSTA_APPSETTINGS.backup" > "$PROPOSTA_APPSETTINGS"
    else
        echo -e "${YELLOW}⚠️  jq não instalado. Atualize manualmente:${NC}"
        echo "  Database.Provider: $PROVIDER"
        echo "  ConnectionString: $CONNECTION_STRING"
    fi
fi

# ContratacaoService
CONTRATACAO_APPSETTINGS="src/ContratacaoService/ContratacaoService.API/appsettings.Development.json"
if [ -f "$CONTRATACAO_APPSETTINGS" ]; then
    cp "$CONTRATACAO_APPSETTINGS" "$CONTRATACAO_APPSETTINGS.backup"

    if command -v jq &> /dev/null; then
        jq ".Database.Provider = \"$PROVIDER\" | .ConnectionStrings.DefaultConnection = \"$CONNECTION_STRING\"" "$CONTRATACAO_APPSETTINGS.backup" > "$CONTRATACAO_APPSETTINGS"
    fi
fi

echo -e "${GREEN}✓${NC} Configurações atualizadas!"

# Migrations
echo ""
echo -e "${YELLOW}⚠️  IMPORTANTE - Migrations:${NC}"
echo ""
echo "Você precisará criar novas migrations para $PROVIDER:"
echo ""
echo "# PropostaService"
echo "cd src/PropostaService/PropostaService.Infrastructure"
echo "dotnet ef migrations add InitialCreate_${PROVIDER} --context PropostaDbContext --output-dir Migrations/${PROVIDER}"
echo ""
echo "# ContratacaoService"
echo "cd src/ContratacaoService/ContratacaoService.Infrastructure"
echo "dotnet ef migrations add InitialCreate_${PROVIDER} --context ContratacaoDbContext --output-dir Migrations/${PROVIDER}"
echo ""

# Docker Compose
if [ "$choice" != "4" ]; then
    echo ""
    read -p "Deseja atualizar docker-compose.yml? (s/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Ss]$ ]]; then
        echo ""
        echo -e "${YELLOW}⚠️  Docker Compose precisa ser atualizado manualmente.${NC}"
        echo "Veja exemplos em DATABASE_PROVIDERS.md"
    fi
fi

echo ""
echo "=========================================="
echo -e "${GREEN}✓${NC} Provedor alterado para: $PROVIDER"
echo "=========================================="
echo ""
echo "Próximos passos:"
echo "1. ✅ Configuração atualizada"
echo "2. 📦 Instalar pacote NuGet (se necessário)"
echo "3. 🔄 Criar migrations para o novo provider"
echo "4. 🐳 Atualizar docker-compose.yml (se usar Docker)"
echo "5. ▶️  Executar: docker-compose up"
echo ""
echo "📚 Consulte DATABASE_PROVIDERS.md para mais detalhes"
echo ""


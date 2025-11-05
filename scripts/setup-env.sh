#!/bin/bash

# Script para configurar arquivos de ambiente

echo "=========================================="
echo "Configuração de Ambientes"
echo "Plataforma de Seguros"
echo "=========================================="

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Função para verificar arquivo de ambiente
check_env_file() {
    local file=$1
    local name=$2

    if [ -f "$file" ]; then
        echo -e "${GREEN}✓${NC} Arquivo $name está configurado: $file"
        return 0
    else
        echo -e "${RED}❌${NC} Arquivo $name não encontrado: $file"
        return 1
    fi
}

# Verificar se estamos no diretório correto
if [ ! -f "PlataformaSeguros.sln" ]; then
    echo -e "${RED}❌ Erro: Execute este script no diretório raiz do projeto${NC}"
    exit 1
fi

echo ""
echo "Escolha o ambiente para configurar:"
echo "1) Development (Desenvolvimento Local)"
echo "2) Homologação"
echo "3) Production (Produção)"
echo "4) Todos"
echo ""
read -p "Opção [1-4]: " choice

case $choice in
    1)
        echo ""
        echo "Verificando ambiente de Desenvolvimento..."
        check_env_file "env.development" "Development"
        echo ""
        echo -e "${GREEN}✓${NC} Ambiente de desenvolvimento pronto para uso!"
        echo ""
        echo "Para usar:"
        echo "  docker-compose up"
        echo "  ou"
        echo "  docker-compose --env-file env.development up"
        ;;
    2)
        echo ""
        echo "Verificando ambiente de Homologação..."
        check_env_file "env.homologacao" "Homologação"
        echo ""
        echo -e "${YELLOW}⚠️  ATENÇÃO:${NC}"
        echo "  1. Edite o arquivo env.homologacao"
        echo "  2. Substitua todos os valores CHANGE_ME"
        echo "  3. Use senhas fortes e únicas"
        echo ""
        echo "Para usar:"
        echo "  docker-compose --env-file env.homologacao up"
        ;;
    3)
        echo ""
        echo "Verificando ambiente de Produção..."
        check_env_file "env.production" "Produção"
        echo ""
        echo -e "${RED}⚠️  IMPORTANTE - PRODUÇÃO:${NC}"
        echo "  1. Edite todos os valores USE_SECRET_MANAGER"
        echo "  2. Use um gerenciador de secrets:"
        echo "     - Azure Key Vault"
        echo "     - AWS Secrets Manager"
        echo "     - HashiCorp Vault"
        echo "  3. Configure secrets no orquestrador (K8s, etc)"
        echo ""
        echo "Para usar:"
        echo "  docker-compose --env-file env.production up"
        ;;
    4)
        echo ""
        echo "Verificando todos os ambientes..."
        check_env_file "env.development" "Development"
        check_env_file "env.homologacao" "Homologação"
        check_env_file "env.production" "Produção"
        echo ""
        echo -e "${GREEN}✓${NC} Verificação completa!"
        ;;
    *)
        echo -e "${RED}❌ Opção inválida${NC}"
        exit 1
        ;;
esac

echo ""
echo "=========================================="
echo "Configuração concluída!"
echo "=========================================="
echo ""
echo "📚 Próximos passos:"
echo ""
echo "1. Leia: ENV_SETUP.md para detalhes"
echo "2. Configure as variáveis necessárias"
echo "3. Execute: docker-compose up"
echo ""


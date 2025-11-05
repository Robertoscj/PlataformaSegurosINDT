#!/bin/bash

# Script para executar um serviço específico
# Uso: ./executar-servico.sh [proposta|contratacao] [run|watch]

set -e

# Cores para output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Função de ajuda
show_help() {
    echo -e "${BLUE}═══════════════════════════════════════════════════════════${NC}"
    echo -e "${GREEN}  Script de Execução dos Serviços${NC}"
    echo -e "${BLUE}═══════════════════════════════════════════════════════════${NC}"
    echo ""
    echo "Uso: ./executar-servico.sh [servico] [modo]"
    echo ""
    echo "Serviços disponíveis:"
    echo "  proposta      - PropostaService (porta 5001)"
    echo "  contratacao   - ContratacaoService (porta 5002)"
    echo "  ambos         - Executa ambos os serviços"
    echo ""
    echo "Modos disponíveis:"
    echo "  run           - Execução normal (dotnet run)"
    echo "  watch         - Execução com hot reload (dotnet watch run)"
    echo ""
    echo "Exemplos:"
    echo "  ./executar-servico.sh proposta watch"
    echo "  ./executar-servico.sh contratacao run"
    echo "  ./executar-servico.sh ambos watch"
    echo ""
}

# Verificar argumentos
if [ $# -lt 2 ]; then
    show_help
    exit 1
fi

SERVICO=$1
MODO=$2

# Diretório raiz do projeto
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Função para executar um serviço
executar_servico() {
    local nome=$1
    local diretorio=$2
    local porta=$3
    
    echo -e "${BLUE}═══════════════════════════════════════════════════════════${NC}"
    echo -e "${GREEN}  Iniciando ${nome}${NC}"
    echo -e "${BLUE}═══════════════════════════════════════════════════════════${NC}"
    echo -e "📁 Diretório: ${diretorio}"
    echo -e "🌐 URL: ${YELLOW}http://localhost:${porta}${NC}"
    echo -e "📚 Swagger: ${YELLOW}http://localhost:${porta}${NC}"
    echo -e "💚 Health Check: ${YELLOW}http://localhost:${porta}/health${NC}"
    echo ""
    
    cd "$diretorio"
    
    if [ "$MODO" == "watch" ]; then
        echo -e "${GREEN}🔄 Modo: Hot Reload (dotnet watch run)${NC}"
        echo ""
        dotnet watch run
    else
        echo -e "${GREEN}▶️  Modo: Normal (dotnet run)${NC}"
        echo ""
        dotnet run
    fi
}

# Executar o serviço solicitado
case $SERVICO in
    proposta)
        PROPOSTA_DIR="$PROJECT_ROOT/src/PropostaService/PropostaService.API"
        executar_servico "PropostaService" "$PROPOSTA_DIR" "5001"
        ;;
    
    contratacao)
        CONTRATACAO_DIR="$PROJECT_ROOT/src/ContratacaoService/ContratacaoService.API"
        executar_servico "ContratacaoService" "$CONTRATACAO_DIR" "5002"
        ;;
    
    ambos)
        echo -e "${BLUE}═══════════════════════════════════════════════════════════${NC}"
        echo -e "${GREEN}  Executando Ambos os Serviços${NC}"
        echo -e "${BLUE}═══════════════════════════════════════════════════════════${NC}"
        echo ""
        echo -e "${YELLOW}💡 Dica: Use o docker-compose para executar ambos os serviços${NC}"
        echo -e "${YELLOW}   Comando: docker-compose up -d${NC}"
        echo ""
        echo -e "📚 URLs dos serviços:"
        echo -e "   PropostaService:    ${YELLOW}http://localhost:5001${NC}"
        echo -e "   ContratacaoService: ${YELLOW}http://localhost:5002${NC}"
        echo ""
        
        # Executar em background
        PROPOSTA_DIR="$PROJECT_ROOT/src/PropostaService/PropostaService.API"
        CONTRATACAO_DIR="$PROJECT_ROOT/src/ContratacaoService/ContratacaoService.API"
        
        if [ "$MODO" == "watch" ]; then
            cd "$PROPOSTA_DIR" && dotnet watch run &
            PROPOSTA_PID=$!
            
            cd "$CONTRATACAO_DIR" && dotnet watch run &
            CONTRATACAO_PID=$!
        else
            cd "$PROPOSTA_DIR" && dotnet run &
            PROPOSTA_PID=$!
            
            cd "$CONTRATACAO_DIR" && dotnet run &
            CONTRATACAO_PID=$!
        fi
        
        echo -e "${GREEN}✅ Serviços iniciados!${NC}"
        echo -e "Para parar, pressione Ctrl+C"
        
        # Aguardar término
        wait $PROPOSTA_PID $CONTRATACAO_PID
        ;;
    
    *)
        echo -e "${RED}❌ Serviço inválido: $SERVICO${NC}"
        echo ""
        show_help
        exit 1
        ;;
esac


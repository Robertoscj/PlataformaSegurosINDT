# 📚 Índice de Documentação - Plataforma de Seguros

## 🚀 Começando


**[QUICK_START.md](QUICK_START.md)** - Guia rápido para executar o projeto

### Para Desenvolvedores
 **[README.md](README.md)** - Documentação principal completa


## 📖 Documentação Técnica

### Arquitetura
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Detalhes da arquitetura hexagonal, DDD, SOLID


## 🏗️ Estrutura do Projeto

```
PlataformaSegurosINDT/
│
├── 📚 Documentação
│   ├── README.md              - Documentação principal
│   ├── QUICK_START.md        - Início rápido
│   ├── ARCHITECTURE.md       - Arquitetura detalhada
│   ├── ENV_SETUP.md          - Configuração de ambientes 🔐
│   ├── DATABASE_PROVIDERS.md - Guia de bancos de dados 🗄️
│   └── INDEX.md              - Este arquivo
│
├── 🐳 Docker
│   ├── docker-compose.yml         - Orquestração completa
│   ├── docker-compose.override.yml - Override para dev
│   └── .dockerignore              - Exclusões do Docker
│
├── 🔐 Configuração de Ambientes
│   ├── env.development   - Configuração para dev
│   ├── env.homologacao   - Configuração para homolog
│   ├── env.production    - Configuração para prod
│   └── ENV_SETUP.md      - Guia de configuração
│
├── 📦 Solução .NET
│   └── PlataformaSeguros.sln - Solution principal
│
├── 💾 Banco de Dados
│   └── database/
│       ├── proposta-init.sql     - Script inicial PropostaDB
│       └── contratacao-init.sql  - Script inicial ContratacaoDB
│
├── 🛠️ Scripts
│   └── scripts/
│       ├── build.sh                    - Build da solução
│       ├── executar-testes.sh          - Executar testes
│       ├── criar-migrations.sh         - Criar migrations
│       ├── aplicar-migrations.sh       - Aplicar migrations
│       ├── setup-env.sh                - Configurar ambientes
│       └── switch-database-provider.sh - Trocar banco de dados 🗄️
│
├── 💻 Código Fonte
│   └── src/
│       ├── PropostaService/      - Microserviço de Propostas
│       │   ├── Domain/           - Camada de Domínio
│       │   ├── Application/      - Casos de Uso
│       │   ├── Infrastructure/   - Adapters (DB)
│       │   └── API/             - Controllers REST
│       │
│       └── ContratacaoService/   - Microserviço de Contratação
│           ├── Domain/
│           ├── Application/
│           ├── Infrastructure/
│           └── API/
│
└── 🧪 Testes
    └── tests/
        ├── PropostaService.Tests/
        └── ContratacaoService.Tests/
```

## 🎯 Guia

###  Quero  executar
1. **[QUICK_START.md](QUICK_START.md)** - Apenas isso!

## 📊 Checklist de Avaliação

Use este checklist para avaliar o projeto:

### Requisitos Funcionais
- [ ] PropostaService está rodando?
- [ ] ContratacaoService está rodando?
- [ ] Consegue criar proposta?
- [ ] Consegue listar propostas?
- [ ] Consegue alterar status?
- [ ] Consegue contratar proposta aprovada?
- [ ] Não permite contratar proposta não aprovada?
- [ ] Swagger está funcionando?

### Requisitos Técnicos
- [ ] Arquitetura Hexagonal implementada?
- [ ] Clean Architecture seguida?
- [ ] DDD aplicado (Entidades, VOs)?
- [ ] SOLID respeitado?
- [ ] Testes unitários presentes?
- [ ] Docker configurado?
- [ ] Banco de dados PostgreSQL?
- [ ] Migrations funcionando?
- [ ] Comunicação HTTP entre serviços?

### Qualidade de Código
- [ ] Código limpo e legível?
- [ ] Nomenclatura clara?
- [ ] Validações no lugar certo?
- [ ] Sem código duplicado?
- [ ] Logs apropriados?
- [ ] Exceptions bem tratadas?

### Documentação
- [ ] README completo?
- [ ] Exemplos de uso?
- [ ] Diagramas de arquitetura?
- [ ] Instruções de execução claras?

## 🔗 Links Rápidos

### Executar
```bash
# Docker
docker-compose up --build

# Local
dotnet run --project src/PropostaService/PropostaService.API
dotnet run --project src/ContratacaoService/ContratacaoService.API
```

### Testar
```bash
# Todos os testes
dotnet test

# Com script
./scripts/executar-testes.sh
```

### Acessar
- PropostaService Swagger: http://localhost:5001/swagger
- ContratacaoService Swagger: http://localhost:5002/swagger
- PropostaService Health: http://localhost:5001/health
- ContratacaoService Health: http://localhost:5002/health

## 📞 Suporte

### Problemas Comuns
- **Erro ao restaurar pacotes**: Veja [QUICK_START.md](QUICK_START.md#-solução-de-problemas)
- **Porta em uso**: Mude em `docker-compose.yml`
- **Banco não conecta**: Verifique se PostgreSQL está rodando

### Onde Procurar
- **Como executar**: [QUICK_START.md](QUICK_START.md)
- **Como funciona**: [ARCHITECTURE.md](ARCHITECTURE.md)
- **Como usar**: [API_EXAMPLES.md](API_EXAMPLES.md)
- **Como contribuir**: [CONTRIBUTING.md](CONTRIBUTING.md)

## 🎓 Conceitos Abordados

Este projeto demonstra:

### Arquitetura
✅ Hexagonal (Ports & Adapters)  
✅ Clean Architecture  
✅ Microservices  
✅ RESTful APIs  

### Design
✅ Domain-Driven Design (DDD)  
✅ SOLID Principles  
✅ Design Patterns  
✅ Separation of Concerns  

### Qualidade
✅ Clean Code  
✅ Unit Testing  
✅ Code Documentation  
✅ API Documentation  

### DevOps
✅ Docker  
✅ Docker Compose  
✅ Database Migrations  
✅ Shell Scripts  

## 🏆 Highlights

| Aspecto | Implementação |
|---------|---------------|
| **Arquitetura** | Hexagonal pura com camadas bem definidas |
| **Domínio** | Entidades ricas + Value Objects imutáveis |
| **Validações** | CPF com dígitos verificadores completos |
| **Testes** | xUnit + Moq + FluentAssertions |
| **Docs** | Completa com diagramas e exemplos |
| **Docker** | Pronto para produção |

## 📈 Métricas

- **Projetos**: 10 (8 principais + 2 testes)
- **Microserviços**: 2 independentes
- **Bancos de Dados**: 2 PostgreSQL isolados
- **Testes Unitários**: 15+ testes
- **Linhas de Código**: ~3.500 (aprox.)
- **Documentos**: 7 arquivos .md
- **Tempo de Dev**: 8-12 horas estimadas

---

**Navegação Rápida:**

🎯 [Entrega](ENTREGA.md) | 
📖 [README](README.md) | 
🏗️ [Arquitetura](ARCHITECTURE.md) | 
🚀 [Quick Start](QUICK_START.md) | 
📡 [API Examples](API_EXAMPLES.md)




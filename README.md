# Plataforma de Seguros - Teste Técnico

Sistema de gerenciamento de propostas e contratações de seguros desenvolvido com **Arquitetura Hexagonal (Ports & Adapters)**, **Clean Architecture**, **DDD** e **Microserviços**.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Pré-requisitos](#pré-requisitos)
- [Como Executar](#como-executar)
- [Endpoints da API](#endpoints-da-api)
- [Testes](#testes)
- [Decisões de Projeto](#decisões-de-projeto)

## 🎯 Visão Geral

O sistema é composto por dois microserviços independentes:

### 1. **PropostaService**
Responsável pelo gerenciamento de propostas de seguro:
- ✅ Criar proposta de seguro
- ✅ Listar propostas (com filtro por status)
- ✅ Consultar proposta específica
- ✅ Alterar status da proposta (Em Análise, Aprovada, Rejeitada)

### 2. **ContratacaoService**
Responsável pela contratação de propostas aprovadas:
- ✅ Contratar uma proposta (somente se Aprovada)
- ✅ Listar contratações
- ✅ Consultar contratação específica
- ✅ Comunicação com PropostaService via HTTP

## 🏗️ Arquitetura

### Arquitetura Hexagonal (Ports & Adapters)

O projeto segue rigorosamente a Arquitetura Hexagonal, dividindo cada microserviço em camadas:

```
┌─────────────────────────────────────────────────┐
│                API Layer (Adapter)              │
│  Controllers, Middleware, Configuration         │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│           Application Layer (Use Cases)         │
│  CriarProposta, ListarPropostas, Contratar      │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│          Domain Layer (Core Business)           │
│  Entities, Value Objects, Domain Services       │
│         Ports (Interfaces/Contracts)            │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│       Infrastructure Layer (Adapters)           │
│  Repositories, External Services, Database      │
└─────────────────────────────────────────────────┘
```

### Camadas de cada Microserviço

#### **Domain** (Núcleo - Regras de Negócio)
- **Entities**: Proposta, Contratacao
- **Value Objects**: CPF, ValorMonetario
- **Enums**: StatusProposta
- **Ports**: Interfaces (IPropostaRepository, IContratacaoRepository, IPropostaServiceClient)

#### **Application** (Casos de Uso)
- Use Cases que orquestram a lógica de negócio
- DTOs para comunicação com a camada externa
- Sem dependência de infraestrutura

#### **Infrastructure** (Adapters)
- Implementação dos Ports
- Entity Framework Core + PostgreSQL
- HttpClient para comunicação entre serviços
- Configurações de persistência

#### **API** (Apresentação)
- Controllers REST
- Configuração de DI (Dependency Injection)
- Swagger/OpenAPI
- Health Checks

### Comunicação entre Microserviços

O sistema suporta **dois tipos de comunicação**:

#### 1. **Síncrona** (HTTP REST)
```
┌─────────────────┐         HTTP REST         ┌──────────────────┐
│                 │ ─────────────────────────> │                  │
│ Contratacao     │   GET /api/propostas/{id}  │   Proposta       │
│    Service      │ <───────────────────────── │    Service       │
│                 │      PropostaDTO           │                  │
└─────────────────┘                            └──────────────────┘
```

#### 2. **Assíncrona** (Mensageria AWS)
```
┌─────────────────┐                            ┌──────────────────┐
│   Proposta      │   Publica Evento (SNS)     │                  │
│    Service      │ ───────────────┐           │                  │
└─────────────────┘                │           │                  │
                                   ▼           │                  │
                            ┌──────────┐       │                  │
                            │ AWS SNS  │       │                  │
                            │  Topic   │       │                  │
                            └────┬─────┘       │                  │
                                 │             │                  │
                                 ▼             │                  │
                            ┌──────────┐       │                  │
                            │ AWS SQS  │       │                  │
                            │  Queue   │       │                  │
                            └────┬─────┘       │                  │
                                 │             │  Contratacao     │
                                 └────────────>│    Service       │
                                Consome Evento │                  │
                                               └──────────────────┘
```

**Quando uma proposta é aprovada:**
1. PropostaService publica evento `PropostaAprovada` no SNS
2. SNS encaminha para fila SQS
3. ContratacaoService consome automaticamente
4. Processamento assíncrono e desacoplado

📚 **Veja mais:** [MENSAGERIA.md](MENSAGERIA.md)

## 🛠️ Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **C# 12** - Linguagem
- **AWS SDK** - SNS e SQS para mensageria assíncrona
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - Banco de dados relacional
- **Docker & Docker Compose** - Containerização
- **xUnit** - Framework de testes
- **Moq** - Mock para testes unitários
- **FluentAssertions** - Assertions expressivas
- **Swagger/OpenAPI** - Documentação de API

## 📁 Estrutura do Projeto

```
PlataformaSegurosINDT/
├── src/
│   ├── PropostaService/
│   │   ├── PropostaService.Domain/          # Entidades, Value Objects, Ports
│   │   ├── PropostaService.Application/     # Use Cases, DTOs
│   │   ├── PropostaService.Infrastructure/  # Repositories, DbContext
│   │   └── PropostaService.API/            # Controllers, Program.cs
│   │
│   └── ContratacaoService/
│       ├── ContratacaoService.Domain/
│       ├── ContratacaoService.Application/
│       ├── ContratacaoService.Infrastructure/
│       └── ContratacaoService.API/
│
├── tests/
│   ├── PropostaService.Tests/              # Testes unitários
│   └── ContratacaoService.Tests/
│
├── database/                                # Scripts SQL
├── scripts/                                 # Scripts de automação
├── docker-compose.yml                       # Orquestração de containers
└── README.md
```

## ⚙️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) e Docker Compose
- [PostgreSQL](https://www.postgresql.org/download/) (opcional, se não usar Docker)

## 🔐 Configuração de Ambientes

O projeto utiliza variáveis de ambiente para configuração. Arquivos disponíveis:

- `env.development` - Para desenvolvimento local
- `env.homologacao` - Para ambiente de homologação  
- `env.production` - Para ambiente de produção

**Leia [ENV_SETUP.md](ENV_SETUP.md) para instruções detalhadas de configuração.**

## 🚀 Como Executar

### Opção 1: Usando Docker (Recomendado)

```bash
# 1. Clone o repositório
git clone <repository-url>
cd PlataformaSegurosINDT

# 2. Execute com Docker Compose
docker-compose up --build

# Os serviços estarão disponíveis em:
# - PropostaService: http://localhost:5001
# - ContratacaoService: http://localhost:5002
# - Swagger PropostaService: http://localhost:5001/swagger
# - Swagger ContratacaoService: http://localhost:5002/swagger
```

### Opção 2: Execução Local

```bash
# 1. Configure o PostgreSQL localmente
# Crie os bancos: propostadb e contratacaodb

# 2. Execute os scripts de banco (opcional)
psql -U postgres -d propostadb -f database/proposta.sql
psql -U postgres -d contratacaodb -f database/contratacao.sql

# 3. Execute as migrations (o EF fará automaticamente no startup em Dev)
# Ou manualmente:
chmod +x scripts/criar-migrations.sh
./scripts/criar-migrations.sh

chmod +x scripts/aplicar-migrations.sh
./scripts/aplicar-migrations.sh

# 4. Execute os serviços
# Terminal 1 - PropostaService
cd src/PropostaService/PropostaService.API
dotnet run

# Terminal 2 - ContratacaoService
cd src/ContratacaoService/ContratacaoService.API
dotnet run
```

### Configuração de Ambiente

Ajuste os arquivos `appsettings.json` se necessário:

**PropostaService.API/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=propostadb;Username=postgres;Password=postgres"
  }
}
```

**ContratacaoService.API/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=contratacaodb;Username=postgres;Password=postgres"
  },
  "PropostaService": {
    "BaseUrl": "http://localhost:5001"
  }
}
```

## 📡 Endpoints da API

### PropostaService (http://localhost:5001)

#### POST /api/propostas
Cria uma nova proposta de seguro.

**Request Body:**
```json
{
  "nomeCliente": "João da Silva",
  "cpfCliente": "123.456.789-09",
  "tipoSeguro": "Vida",
  "valorCobertura": 100000.00,
  "valorPremio": 500.00
}
```

**Response:** 201 Created
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nomeCliente": "João da Silva",
  "cpfCliente": "12345678909",
  "tipoSeguro": "Vida",
  "valorCobertura": 100000.00,
  "valorPremio": 500.00,
  "status": 1,
  "dataCriacao": "2024-01-15T10:30:00Z",
  "dataAtualizacao": null
}
```

**Tipos de Seguro Válidos:** Vida, Auto, Residencial, Empresarial, Viagem

**Status:**
- `1` = Em Análise
- `2` = Aprovada
- `3` = Rejeitada

#### GET /api/propostas
Lista todas as propostas.

**Query Parameters:**
- `status` (opcional): Filtrar por status (1, 2 ou 3)

**Response:** 200 OK
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nomeCliente": "João da Silva",
    "cpfCliente": "12345678909",
    "tipoSeguro": "Vida",
    "valorCobertura": 100000.00,
    "valorPremio": 500.00,
    "status": 1,
    "dataCriacao": "2024-01-15T10:30:00Z",
    "dataAtualizacao": null
  }
]
```

#### GET /api/propostas/{id}
Obtém uma proposta específica.

**Response:** 200 OK ou 404 Not Found

#### PATCH /api/propostas/{id}/status
Altera o status de uma proposta.

**Request Body:**
```json
{
  "novoStatus": 2
}
```

**Response:** 200 OK

### ContratacaoService (http://localhost:5002)

#### POST /api/contratacoes
Contrata uma proposta aprovada.

**Request Body:**
```json
{
  "propostaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "dataVigenciaInicio": "2024-02-01T00:00:00Z",
  "dataVigenciaFim": "2025-02-01T00:00:00Z"
}
```

**Response:** 201 Created
```json
{
  "id": "9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d",
  "propostaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "numeroApolice": "APO-20240115-A1B2C3",
  "dataContratacao": "2024-01-15T14:20:00Z",
  "dataVigenciaInicio": "2024-02-01T00:00:00Z",
  "dataVigenciaFim": "2025-02-01T00:00:00Z",
  "valorPremio": 500.00
}
```

**Regras:**
- A proposta deve existir no PropostaService
- A proposta deve estar com status `Aprovada` (2)
- Não pode haver outra contratação para a mesma proposta

#### GET /api/contratacoes
Lista todas as contratações.

**Response:** 200 OK

#### GET /api/contratacoes/{id}
Obtém uma contratação específica.

**Response:** 200 OK ou 404 Not Found

## 🧪 Testes

O projeto inclui testes unitários abrangentes para garantir a qualidade do código.

### Executar todos os testes

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Testes Implementados

#### PropostaService.Tests
- ✅ Testes de Value Objects (CPF, ValorMonetario)
- ✅ Testes de Entidades (Proposta)
- ✅ Testes de Use Cases (CriarPropostaUseCase)

#### ContratacaoService.Tests
- ✅ Testes de Entidades (Contratacao)
- ✅ Testes de Use Cases (ContratarPropostaUseCase)

### Exemplo de execução

```bash
cd PlataformaSegurosINDT

# Testar PropostaService
dotnet test tests/PropostaService.Tests/

# Testar ContratacaoService
dotnet test tests/ContratacaoService.Tests/
```

## 🎯 Decisões de Projeto

### 1. Arquitetura Hexagonal
- **Por quê?** Separação clara entre domínio e infraestrutura, facilitando manutenção e testes
- **Benefício:** Fácil substituição de adapters (ex: trocar PostgreSQL por MongoDB)

### 2. Clean Architecture + DDD
- **Domain-Driven Design:** Entidades ricas com comportamento
- **Value Objects:** CPF e ValorMonetario com validações embutidas
- **Agregados:** Proposta e Contratacao como raízes de agregados

### 3. SOLID
- **SRP:** Cada classe tem uma responsabilidade única
- **OCP:** Extensível via interfaces (Ports)
- **LSP:** Substituibilidade de implementações
- **ISP:** Interfaces segregadas (IPropostaRepository, IContratacaoRepository)
- **DIP:** Dependência de abstrações, não de implementações concretas

### 4. Padrões Utilizados
- **Repository Pattern:** Abstração de acesso a dados
- **Use Case Pattern:** Encapsulamento de lógica de aplicação
- **DTO Pattern:** Separação entre modelo de domínio e API
- **Factory Pattern:** Métodos estáticos `Criar()` nas entidades

### 5. Comunicação entre Microserviços
- **HTTP REST:** Simples e direto para este caso de uso
- **Alternativas consideradas:** Mensageria (RabbitMQ, Kafka) para maior resiliência
- **Trade-off:** HTTP é mais simples, mas menos resiliente que mensageria assíncrona

### 6. Banco de Dados
- **PostgreSQL:** Robusto, open-source, excelente para dados relacionais
- **Um banco por serviço:** Isolamento completo entre microserviços
- **Migrations:** Versionamento automático do schema

### 7. Testes
- **xUnit:** Framework moderno e performático
- **Moq + FluentAssertions:** Testes legíveis e expressivos
- **Foco em testes unitários:** Validação de regras de negócio

## 📊 Diagrama de Arquitetura

```
┌─────────────────────────────────────────────────────────────────┐
│                         EXTERNAL WORLD                          │
│                    (HTTP Clients, Browser)                      │
└────────────┬────────────────────────────────┬───────────────────┘
             │                                │
             │ HTTP                           │ HTTP
             ▼                                ▼
┌────────────────────────┐        ┌────────────────────────┐
│  PropostaService.API   │        │ ContratacaoService.API │
│   (Port 5001/80)       │        │   (Port 5002/80)       │
│  ┌──────────────────┐  │        │  ┌──────────────────┐  │
│  │   Controllers    │  │        │  │   Controllers    │  │
│  │   Swagger UI     │  │        │  │   Swagger UI     │  │
│  └────────┬─────────┘  │        │  └────────┬─────────┘  │
│           │             │        │           │             │
│  ┌────────▼─────────┐  │        │  ┌────────▼─────────┐  │
│  │   Use Cases      │  │        │  │   Use Cases      │  │
│  │  - CriarProposta │  │        │  │ - ContratarProp. │  │
│  │  - ListarPropst  │  │        │  │ - ListarContrat. │  │
│  │  - AlterarStatus │  │        │  │                  │  │
│  └────────┬─────────┘  │        │  └────┬────────┬────┘  │
│           │             │        │       │        │        │
│  ┌────────▼─────────┐  │        │  ┌────▼──┐  ┌──▼─────┐ │
│  │     Domain       │  │        │  │Domain │  │External│ │
│  │  - Proposta      │  │        │  │Contrat│  │Service │ │
│  │  - CPF (VO)      │  │        │  │       │  │ Client │ │
│  │  - ValorMon.(VO) │  │        │  │       │  └────┬───┘ │
│  └────────┬─────────┘  │        │  └───────┘       │     │
│           │             │        │                  │     │
│  ┌────────▼─────────┐  │        │  ┌───────────────▼───┐ │
│  │ Infrastructure   │  │        │  │ Infrastructure    │ │
│  │  - Repository    │  │        │  │  - Repository     │ │
│  │  - DbContext     │  │        │  │  - DbContext      │ │
│  └────────┬─────────┘  │        │  │  - HTTP Client    │ │
└───────────┼─────────────┘        │  └────────┬──────────┘ │
            │                      └───────────┼─────────────┘
            │                                  │
            │ SQL                              │ SQL
            ▼                                  ▼
┌────────────────────┐            ┌────────────────────┐
│   proposta-db      │            │  contratacao-db    │
│   PostgreSQL       │            │   PostgreSQL       │
│   (Port 5432)      │            │   (Port 5433)      │
└────────────────────┘            └────────────────────┘

        │                                     │
        └──────────────┬──────────────────────┘
                       │
                       │ Docker Network
                       │ (seguros-network)
                       │
```

## 🗄️ Suporte a Múltiplos Bancos de Dados

O projeto suporta facilmente a troca entre diferentes provedores de banco de dados:

- ✅ **PostgreSQL** (Padrão)
- ✅ **SQL Server**
- ✅ **MySQL / MariaDB**
- ✅ **InMemory** (Testes)

Para trocar de provedor, basta alterar a configuração:

```json
{
  "Database": {
    "Provider": "SqlServer"  // ou PostgreSQL, MySql, InMemory
  },
  "ConnectionStrings": {
    "DefaultConnection": "sua-connection-string"
  }
}
```

**Script helper:**
```bash
./scripts/switch-database-provider.sh
```

📖 **Guia completo:** [DATABASE_PROVIDERS.md](DATABASE_PROVIDERS.md)

## 🔒 Validações e Regras de Negócio

### Proposta
- Nome do cliente: mínimo 3 caracteres
- CPF: validação com dígitos verificadores
- Tipo de seguro: apenas valores válidos (Vida, Auto, Residencial, Empresarial, Viagem)
- Valores: não podem ser negativos
- Status inicial: sempre "Em Análise"
- Não pode alterar status de proposta já finalizada (Aprovada/Rejeitada)

### Contratação
- Proposta deve existir
- Proposta deve estar aprovada
- Data de vigência: início deve ser anterior ao fim
- Data de vigência: não pode ser no passado
- Valor do prêmio: deve ser maior que zero
- Apenas uma contratação por proposta

## 📝 Melhorias Futuras

- [ ] Implementar autenticação e autorização (JWT)
- [ ] Adicionar mensageria (RabbitMQ) para comunicação assíncrona
- [ ] Implementar padrão Circuit Breaker (Polly) para resiliência
- [ ] Adicionar API Gateway (Ocelot)
- [ ] Implementar testes de integração
- [ ] Adicionar observabilidade (Logging, Metrics, Tracing)
- [ ] Implementar CQRS para queries complexas
- [ ] Adicionar cache (Redis) para melhorar performance
- [ ] CI/CD com GitHub Actions
- [ ] Monitoramento com Prometheus + Grafana

## 👨‍💻 Autor

Desenvolvido como teste técnico demonstrando conhecimentos em:
- ✅ Arquitetura Hexagonal (Ports & Adapters)
- ✅ Clean Architecture
- ✅ Domain-Driven Design (DDD)
- ✅ SOLID Principles
- ✅ Microservices
- ✅ .NET 8 / C#
- ✅ Entity Framework Core
- ✅ PostgreSQL
- ✅ Docker
- ✅ Testes Unitários
- ✅ RESTful APIs

## 📄 Licença

Este projeto foi desenvolvido por **Roberto Carlos da Silva**, Desenvolvedor Sênior .NET.


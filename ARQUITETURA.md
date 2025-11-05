# Documentação de Arquitetura

## Arquitetura Hexagonal - Detalhamento

### Conceitos Fundamentais

A **Arquitetura Hexagonal** (também conhecida como Ports & Adapters) foi criada por Alistair Cockburn. O objetivo principal é isolar a lógica de negócio (domínio) das preocupações externas (infraestrutura, UI, banco de dados).

### Estrutura em Camadas

```
                    OUTSIDE WORLD
                         ↕
              ┌──────────────────────┐
              │   ADAPTERS (Input)   │
              │  API, Controllers    │
              └──────────┬───────────┘
                         ↓
              ┌──────────────────────┐
              │   APPLICATION LAYER  │
              │     Use Cases        │
              └──────────┬───────────┘
                         ↓
              ┌──────────────────────┐
              │    DOMAIN LAYER      │
              │   Business Logic     │
              │      ← PORTS →       │
              └──────────┬───────────┘
                         ↓
              ┌──────────────────────┐
              │  ADAPTERS (Output)   │
              │ Repositories, APIs   │
              └──────────────────────┘
                         ↕
                  OUTSIDE WORLD
```

## Implementação nos Microserviços

### PropostaService

#### Domain Layer (Core)

**Entidades:**
- `Proposta`: Raiz do agregado com regras de negócio
  - Validação de dados
  - Transições de estado
  - Regras de status

**Value Objects:**
- `Cpf`: Validação completa de CPF com dígitos verificadores
- `ValorMonetario`: Garantia de valores não negativos

**Ports (Interfaces):**
- `IPropostaRepository`: Contrato para persistência

#### Application Layer

**Use Cases:**
- `CriarPropostaUseCase`: Orquestra criação de proposta
- `ListarPropostasUseCase`: Lista com filtros
- `ObterPropostaUseCase`: Busca individual
- `AlterarStatusPropostaUseCase`: Transições de estado

**DTOs:**
- `CriarPropostaRequest`
- `PropostaResponse`
- `AlterarStatusRequest`

#### Infrastructure Layer (Adapters)

**Implementações:**
- `PropostaRepository`: Adapter para Entity Framework
- `PropostaDbContext`: Configuração do banco
- `PropostaConfiguration`: Mapeamento ORM
- `DatabaseProviderFactory`: Factory para criar providers de banco
- `IDatabaseProvider`: Interface para abstração de bancos de dados

**Database Providers (Strategy Pattern):**
- `PostgreSqlProvider`: Adapter para PostgreSQL
- `SqlServerProvider`: Adapter para SQL Server
- `MySqlProvider`: Adapter para MySQL
- `InMemoryProvider`: Adapter para testes

#### API Layer (Adapter de Entrada)

**Controllers:**
- `PropostasController`: Endpoints REST
  - POST /api/propostas
  - GET /api/propostas
  - GET /api/propostas/{id}
  - PATCH /api/propostas/{id}/status

### ContratacaoService

#### Domain Layer

**Entidades:**
- `Contratacao`: Raiz do agregado
  - Geração automática de número de apólice
  - Validação de vigência
  - Validação de valor

**Models:**
- `PropostaDto`: Representação de dados externos

**Ports:**
- `IContratacaoRepository`: Persistência
- `IPropostaServiceClient`: Comunicação externa

#### Application Layer

**Use Cases:**
- `ContratarPropostaUseCase`: Orquestra contratação
  - Verifica duplicidade
  - Valida proposta no serviço externo
  - Verifica aprovação
  - Cria contratação

- `ListarContratacoesUseCase`
- `ObterContratacaoUseCase`

#### Infrastructure Layer

**Adapters:**
- `ContratacaoRepository`: Persistência
- `PropostaServiceClient`: HTTP Client para comunicação entre serviços

**Comunicação HTTP:**
```csharp
public class PropostaServiceClient : IPropostaServiceClient
{
    private readonly HttpClient _httpClient;
    
    public async Task<PropostaDto?> ObterPropostaAsync(Guid id)
    {
        // Chamada HTTP para PropostaService
        var response = await _httpClient.GetAsync($"api/propostas/{id}");
        // ...
    }
}
```

## Fluxo de Dados

### Criação de Proposta

```
1. Cliente HTTP → POST /api/propostas (CriarPropostaRequest)
                  ↓
2. PropostasController recebe e valida
                  ↓
3. Chama CriarPropostaUseCase.ExecutarAsync()
                  ↓
4. Use Case cria entidade Proposta (validações do domínio)
                  ↓
5. Chama IPropostaRepository.CriarAsync() (Port)
                  ↓
6. PropostaRepository implementa (Adapter)
                  ↓
7. Entity Framework persiste no PostgreSQL
                  ↓
8. Retorna PropostaResponse ao cliente
```

### Contratação de Proposta

```
1. Cliente HTTP → POST /api/contratacoes (ContratarPropostaRequest)
                  ↓
2. ContratacoesController
                  ↓
3. ContratarPropostaUseCase
                  ↓
4. Verifica duplicidade via IContratacaoRepository
                  ↓
5. Busca proposta via IPropostaServiceClient (HTTP)
                  ↓
6. PropostaServiceClient faz GET para PropostaService
                  ↓
7. Valida status da proposta (Aprovada?)
                  ↓
8. Cria entidade Contratacao
                  ↓
9. Persiste via IContratacaoRepository
                  ↓
10. Retorna ContratacaoResponse
```

## Princípios SOLID Aplicados

### Single Responsibility Principle (SRP)
- Cada Use Case tem uma única responsabilidade
- Controllers apenas roteiam requisições
- Repositories apenas persistem dados

### Open/Closed Principle (OCP)
- Fácil adicionar novos adapters (ex: MongoDB) sem mudar domínio
- Novos Use Cases podem ser adicionados sem modificar existentes

### Liskov Substitution Principle (LSP)
- Qualquer implementação de IPropostaRepository pode substituir outra
- Mock repositories nos testes substituem implementação real

### Interface Segregation Principle (ISP)
- Interfaces pequenas e específicas
- `IPropostaRepository` não força métodos desnecessários
- `IPropostaServiceClient` com apenas o necessário

### Dependency Inversion Principle (DIP)
- Application depende de abstrações (Ports)
- Infrastructure implementa abstrações
- Inversão de controle via Dependency Injection

## Padrões de Projeto

### Repository Pattern
Abstração da camada de dados:
```csharp
public interface IPropostaRepository
{
    Task<Proposta> CriarAsync(Proposta proposta);
    Task<Proposta?> ObterPorIdAsync(Guid id);
    // ...
}
```

### Strategy Pattern
Abstração de provedores de banco de dados:
```csharp
public interface IDatabaseProvider
{
    string ProviderName { get; }
    void Configure(DbContextOptionsBuilder options, string connectionString);
    bool IsAvailable();
}

// Implementações:
// - PostgreSqlProvider
// - SqlServerProvider
// - MySqlProvider
// - InMemoryProvider
```

### Factory Pattern
**Criação de entidades com validação:**
```csharp
public static Proposta Criar(
    string nomeCliente,
    string cpfCliente,
    // ...
)
{
    // Validações
    return new Proposta(/* ... */);
}
```

**Criação de database providers:**
```csharp
public static class DatabaseProviderFactory
{
    public static IDatabaseProvider Create(string providerName)
    {
        // Retorna o provider apropriado
        // PostgreSQL, SqlServer, MySql, InMemory
    }
}
```

### Use Case Pattern
Encapsulamento de lógica de aplicação:
```csharp
public class CriarPropostaUseCase
{
    private readonly IPropostaRepository _repository;
    
    public async Task<PropostaResponse> ExecutarAsync(
        CriarPropostaRequest request)
    {
        // Lógica do caso de uso
    }
}
```

### DTO Pattern
Separação entre modelos de domínio e API:
```csharp
public record CriarPropostaRequest(/* ... */);
public record PropostaResponse(/* ... */);
```

### Adapter Pattern
**Adaptação de diferentes bancos de dados:**
```csharp
public class PostgreSqlProvider : IDatabaseProvider
{
    public void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            npgsqlOptions.CommandTimeout(30);
        });
    }
}

public class SqlServerProvider : IDatabaseProvider
{
    public void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseSqlServer(connectionString, sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 3);
            sqlServerOptions.CommandTimeout(30);
        });
    }
}
```

## Domain-Driven Design (DDD)

### Entidades
Objetos com identidade única (ID):
- `Proposta`
- `Contratacao`

### Value Objects
Objetos sem identidade, definidos por seus valores:
- `Cpf`: Imutável, com validação
- `ValorMonetario`: Imutável, sempre válido

### Agregados
- `Proposta` é raiz do agregado de propostas
- `Contratacao` é raiz do agregado de contratações
- Cada agregado mantém consistência interna

### Linguagem Ubíqua
Termos do negócio refletidos no código:
- `Proposta`, não `ProposalEntity`
- `Contratar`, não `Create`
- `Status`: `EmAnalise`, `Aprovada`, `Rejeitada`

## Benefícios da Arquitetura

### Testabilidade
- Domain isolado, fácil de testar
- Mocks de repositories via interfaces
- Testes sem dependências externas

### Manutenibilidade
- Mudanças na infraestrutura não afetam domínio
- Código organizado e previsível
- Separação de responsabilidades clara

### Escalabilidade
- Microserviços independentes
- Bancos de dados separados
- Fácil adicionar novos serviços

### Flexibilidade
- **Trocar banco de dados**: PostgreSQL ↔ SQL Server ↔ MySQL (via Strategy Pattern)
- **Adicionar cache**: novo adapter sem mudar domínio
- **Comunicação HTTP → Mensageria**: apenas muda adapter
- **Múltiplos providers**: suporte a diferentes bancos sem reescrever código

## Decisões Arquiteturais

### Por que Hexagonal ao invés de N-Tier?
- **Hexagonal**: Domínio no centro, independente de infraestrutura
- **N-Tier**: Acoplamento entre camadas, domínio conhece infraestrutura

### Por que dois bancos de dados?
- Isolamento completo entre serviços
- Cada serviço pode escalar independentemente
- Falha em um não afeta o outro

### Por que HTTP e não mensageria?
- Simplicidade para o escopo atual
- Resposta síncrona necessária para contratação
- Mensageria seria over-engineering neste momento
- Fácil migrar para mensageria futuramente (só trocar o adapter)

### Por que Entity Framework?
- ORM maduro e produtivo
- Migrations automáticas
- Suporte a múltiplos providers (PostgreSQL, SQL Server, MySQL)
- Facilita testes com InMemory database
- Abstração permite trocar de banco facilmente

### Por que Multi-Database Provider?
- **Flexibilidade**: Clientes podem escolher o banco preferido
- **Portabilidade**: Fácil migração entre ambientes
- **Testes**: InMemory para testes rápidos
- **Open/Closed Principle**: Extensível sem modificar código existente
- **Strategy Pattern**: Troca de implementação em tempo de execução

## Métricas de Qualidade

### Cobertura de Testes
- Testes unitários para entidades
- Testes unitários para value objects
- Testes unitários para use cases
- Mocks para isolamento

### Complexidade Ciclomática
- Métodos pequenos e focados
- Validações separadas
- Lógica clara e direta

### Acoplamento
- Baixo acoplamento via interfaces
- Alta coesão dentro de cada camada
- Dependências sempre para abstrações

## Evolução Futura

### Curto Prazo
1. API Gateway (Ocelot)
2. Autenticação JWT
3. Rate Limiting

### Médio Prazo
1. Mensageria (RabbitMQ)
2. Cache (Redis)
3. CQRS para queries complexas

### Longo Prazo
1. Event Sourcing
2. Saga Pattern para transações distribuídas
3. Service Mesh (Istio)

## Suporte a Múltiplos Bancos de Dados

### Implementação (Strategy + Factory Pattern)

O projeto suporta facilmente a troca entre diferentes provedores de banco de dados através de uma camada de abstração:

```
┌─────────────────────────────────────┐
│         Configuration               │
│  Database:Provider = "SqlServer"    │
└────────────┬────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│    DatabaseProviderFactory          │
│  Create(providerName) → Provider    │
└────────────┬────────────────────────┘
             │
             ▼
┌──────────────────────────────────────────────┐
│           IDatabaseProvider                  │
│  ┌─────────────────┐  ┌─────────────────┐  │
│  │ PostgreSQL      │  │ SQL Server      │  │
│  │ Provider        │  │ Provider        │  │
│  └─────────────────┘  └─────────────────┘  │
│  ┌─────────────────┐  ┌─────────────────┐  │
│  │ MySQL           │  │ InMemory        │  │
│  │ Provider        │  │ Provider        │  │
│  └─────────────────┘  └─────────────────┘  │
└──────────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────┐
│       DbContext Configuration       │
│  Configurado com provider escolhido │
└─────────────────────────────────────┘
```

### Uso

**Via Configuração:**
```json
{
  "Database": {
    "Provider": "PostgreSQL"  // ou SqlServer, MySql, InMemory
  },
  "ConnectionStrings": {
    "DefaultConnection": "sua-connection-string"
  }
}
```

**Via Código:**
```csharp
var dbProvider = builder.Configuration.GetValue<string>("Database:Provider");
var databaseProvider = DatabaseProviderFactory.Create(dbProvider);

builder.Services.AddDbContext<PropostaDbContext>(options =>
{
    databaseProvider.Configure(options, connectionString);
});
```

### Provedores Suportados

| Provider | Pacote NuGet | Status |
|----------|--------------|--------|
| PostgreSQL | Npgsql.EntityFrameworkCore.PostgreSQL | ✅ Padrão |
| SQL Server | Microsoft.EntityFrameworkCore.SqlServer | ✅ Suportado |
| MySQL | Pomelo.EntityFrameworkCore.MySql | ✅ Suportado |
| InMemory | Microsoft.EntityFrameworkCore.InMemory | ✅ Testes |

### Benefícios da Abordagem

1. **Zero Coupling**: Domínio não conhece o banco específico
2. **Easy Switch**: Trocar banco = mudar configuração
3. **Testability**: InMemory para testes rápidos
4. **SOLID Compliant**: Open/Closed Principle
5. **Production Ready**: Retry policies e timeouts configurados

📖 **Documentação completa**: [DATABASE_PROVIDERS.md](DATABASE_PROVIDERS.md)

## Referências

- **Hexagonal Architecture**: Alistair Cockburn
- **Clean Architecture**: Robert C. Martin (Uncle Bob)
- **Domain-Driven Design**: Eric Evans
- **Enterprise Integration Patterns**: Gregor Hohpe & Bobby Woolf
- **Design Patterns**: Gang of Four (Strategy, Factory, Adapter)


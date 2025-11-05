# Quick Start Guide

## 🚀 Inicialização Rápida

### Opção 1: Docker (Recomendada - Mais Simples)

```bash
# 1. Entre no diretório do projeto
cd PlataformaSegurosINDT

# 2. Inicie tudo com Docker Compose
docker-compose up --build

# Aguarde alguns minutos para o build e inicialização...

# Pronto! Acesse:
# - PropostaService Swagger: http://localhost:5001/swagger
# - ContratacaoService Swagger: http://localhost:5002/swagger
```

### Opção 2: Execução Local

```bash
# 1. Entre no diretório do projeto
cd PlataformaSegurosINDT

# 2. Instale o PostgreSQL localmente ou via Docker
docker run --name postgres-local -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16

# 3. Restaure as dependências
dotnet restore

# 4. Execute o PropostaService (Terminal 1)
cd src/PropostaService/PropostaService.API
dotnet run

# 5. Execute o ContratacaoService (Terminal 2)
cd src/ContratacaoService/ContratacaoService.API
dotnet run

# As migrations são aplicadas automaticamente na primeira execução
```

## 🔐 Configuração de Ambientes

Os arquivos de configuração já estão prontos:

- **`env.development`** - Pronto para uso local (credenciais padrão)
- **`env.homologacao`** - Edite os valores CHANGE_ME
- **`env.production`** - Edite os valores USE_SECRET_MANAGER

```bash
# Para usar um ambiente específico:
docker-compose --env-file env.homologacao up

# Para desenvolvimento, simplesmente:
docker-compose up
```

📖 **Detalhes:** Leia [ENV_SETUP.md](ENV_SETUP.md)

## 📝 Primeiro Teste

### 1. Criar uma Proposta

Acesse: http://localhost:5001/swagger

Ou via curl:
```bash
curl -X POST http://localhost:5001/api/propostas \
  -H "Content-Type: application/json" \
  -d '{
    "nomeCliente": "João da Silva",
    "cpfCliente": "123.456.789-09",
    "tipoSeguro": "Vida",
    "valorCobertura": 100000.00,
    "valorPremio": 500.00
  }'
```

**Guarde o `id` retornado!**

### 2. Aprovar a Proposta

```bash
curl -X PATCH http://localhost:5001/api/propostas/{SEU_ID_AQUI}/status \
  -H "Content-Type: application/json" \
  -d '{
    "novoStatus": 2
  }'
```

### 3. Contratar a Proposta

Acesse: http://localhost:5002/swagger

Ou via curl:
```bash
curl -X POST http://localhost:5002/api/contratacoes \
  -H "Content-Type: application/json" \
  -d '{
    "propostaId": "{SEU_ID_AQUI}",
    "dataVigenciaInicio": "2024-02-01T00:00:00Z",
    "dataVigenciaFim": "2025-02-01T00:00:00Z"
  }'
```

**Sucesso!** Você criou e contratou uma apólice de seguro! 🎉

## 🧪 Executar Testes

```bash
# Todos os testes
dotnet test

# Apenas PropostaService
dotnet test tests/PropostaService.Tests/

# Apenas ContratacaoService
dotnet test tests/ContratacaoService.Tests/

# Com scripts (se tiver permissão de execução)
./scripts/executar-testes.sh
```

## 📖 Documentação Adicional

- **[README.md](README.md)** - Documentação completa do projeto
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Detalhes da arquitetura hexagonal
- **[ENV_SETUP.md](ENV_SETUP.md)** - Guia de configuração de ambientes
- **[INDEX.md](INDEX.md)** - Índice de toda documentação

## 🐛 Solução de Problemas

### Erro ao restaurar pacotes NuGet

Se aparecer erro NU1301, tente:

```bash
# Limpar cache do NuGet
dotnet nuget locals all --clear

# Tentar restaurar novamente
dotnet restore --no-cache
```

### Portas já em uso

Se as portas 5001/5002 já estiverem em uso, edite:
- `docker-compose.yml` (para Docker)
- `launchSettings.json` em cada projeto API (para execução local)

### Erro de conexão com banco de dados

Verifique se o PostgreSQL está rodando:

```bash
# Se usando Docker Compose
docker ps

# Se usando PostgreSQL local
psql -U postgres -c "SELECT 1"
```

### Erro "EntityFramework.Tools not found"

Instale globalmente:

```bash
dotnet tool install --global dotnet-ef
```

## 📊 Status dos Serviços

Verifique se os serviços estão healthy:

```bash
# PropostaService
curl http://localhost:5001/health

# ContratacaoService
curl http://localhost:5002/health

# Bancos de dados (se usando Docker)
docker ps --filter "name=db"
```

## 🎓 Próximos Passos

1. ✅ Explore os endpoints no Swagger
2. ✅ Teste diferentes cenários (aprovar, rejeitar)
3. ✅ Veja os testes unitários para entender o domínio
4. ✅ Analise a estrutura de arquitetura hexagonal
5. ✅ Experimente modificar e adicionar features

## 💡 Dicas

- Use o Swagger para testar rapidamente os endpoints
- Os IDs são GUIDs, copie-os diretamente das respostas
- Cada proposta só pode ser contratada uma vez
- Apenas propostas aprovadas podem ser contratadas

## 🚀 Build para Produção

```bash
# Build otimizado
dotnet build --configuration Release

# Publicar para deploy
dotnet publish --configuration Release --output ./publish

# Build Docker
docker-compose -f docker-compose.yml build
```

## 📞 Ajuda

Se precisar de ajuda:
1. Verifique a documentação completa no [README.md](README.md)
2. Consulte o guia de ambientes em [ENV_SETUP.md](ENV_SETUP.md)
3. Revise a arquitetura em [ARCHITECTURE.md](ARCHITECTURE.md)

---

**Boa jornada! 🚀**

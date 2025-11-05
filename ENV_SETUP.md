# 🔐 Guia de Configuração de Ambientes

## 📋 Visão Geral

Este documento explica como configurar e usar as variáveis de ambiente para cada ambiente.

## 🗂️ Arquivos de Ambiente

| Arquivo | Uso | Commitado? |
|---------|-----|------------|
| `env.development` | Desenvolvimento local | ✅ Sim (sem secrets) |
| `env.homologacao` | Ambiente de homologação | ⚠️ Opcional (sem secrets reais) |
| `env.production` | Ambiente de produção | ⚠️ Template apenas |

## 🚀 Setup Inicial

### Para Desenvolvimento Local

```bash
# O arquivo env.development já está pronto para uso!

# 1. Execute com Docker Compose
docker-compose --env-file env.development up --build

# Ou simplesmente:
docker-compose up --build
```

### Para Homologação

```bash
# 1. Edite o arquivo env.homologacao
nano env.homologacao

# 2. Substitua os valores CHANGE_ME com valores reais
# IMPORTANTE: Use um gerenciador de secrets (Vault, etc.)

# 3. Execute no servidor de homologação
docker-compose --env-file env.homologacao up -d
```

### Para Produção

```bash
# IMPORTANTE: NUNCA use arquivo .env em produção!
# Use um gerenciador de secrets:
# - Azure Key Vault
# - AWS Secrets Manager
# - HashiCorp Vault
# - Kubernetes Secrets

# Configure as variáveis diretamente no orquestrador:
# - Kubernetes ConfigMap/Secrets
# - Docker Swarm Secrets
# - Azure App Configuration
# - AWS Systems Manager Parameter Store
```

## 🔑 Variáveis Críticas

### Banco de Dados

```bash
# Development
PROPOSTA_DB_CONNECTION_STRING=Host=localhost;Port=5432;Database=propostadb;Username=postgres;Password=postgres

# Production (usar secret manager!)
PROPOSTA_DB_CONNECTION_STRING=Host=prod-db.rds.amazonaws.com;Port=5432;Database=propostadb;Username=app_user;Password=${SECRET_DB_PASSWORD};SslMode=Require
```

### Comunicação entre Serviços

```bash
# Development
PROPOSTA_SERVICE_BASE_URL=http://localhost:5001

# Docker Compose
PROPOSTA_SERVICE_BASE_URL=http://proposta-api:80

# Kubernetes
PROPOSTA_SERVICE_BASE_URL=http://proposta-api.default.svc.cluster.local
```

### JWT (quando implementado)

```bash
# NUNCA use uma chave fraca ou padrão em produção!
# Development
JWT_SECRET_KEY=development_key_only_min_32_chars_12345678

# Production (usar secret manager!)
JWT_SECRET_KEY=${SECRET_JWT_KEY}  # Mínimo 64 caracteres
```

## 📝 Como Usar com Docker Compose

### Opção 1: Desenvolvimento (Padrão)

```bash
# Usa env.development automaticamente via docker-compose.override.yml
docker-compose up
```

### Opção 2: Arquivo Específico

```bash
# Homologação
docker-compose --env-file env.homologacao up

# Produção
docker-compose --env-file env.production up
```

### Opção 3: Variáveis de Ambiente Inline

```bash
# Definir no terminal
export PROPOSTA_DB_PASSWORD=minha_senha
docker-compose up
```

## 🔒 Boas Práticas de Segurança

### ✅ FAZER

1. **Use gerenciadores de secrets em produção**
   - Azure Key Vault
   - AWS Secrets Manager
   - HashiCorp Vault

2. **Rotate credenciais regularmente**
   ```bash
   # A cada 90 dias no mínimo
   ```

3. **Use diferentes credenciais por ambiente**
   ```bash
   # dev != homolog != prod
   ```

4. **Habilite SSL/TLS em produção**
   ```bash
   REQUIRE_SSL=true
   SslMode=Require
   ```

5. **Limite acesso ao banco**
   ```bash
   # Use usuário com privilégios mínimos
   GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE propostas TO app_user;
   ```

6. **Use secrets do Kubernetes**
   ```yaml
   apiVersion: v1
   kind: Secret
   metadata:
     name: db-credentials
   type: Opaque
   data:
     password: <base64-encoded>
   ```

### ❌ NÃO FAZER

1. ❌ Commitar arquivos .env com senhas reais
2. ❌ Usar senhas fracas (postgres, admin, 123456)
3. ❌ Compartilhar credenciais entre ambientes
4. ❌ Logar senhas em produção
5. ❌ Deixar Swagger habilitado em produção sem auth
6. ❌ Usar HTTP em produção (sempre HTTPS)

## 🎯 Configuração por Ambiente

### Development

```bash
# Foco: Produtividade e debugging
ENABLE_SWAGGER=true
ENABLE_DETAILED_ERRORS=true
LOG_SQL_QUERIES=true
ENABLE_AUTO_MIGRATIONS=true
CORS_ALLOWED_ORIGINS=*
```

### Homologação

```bash
# Foco: Teste e validação
ENABLE_SWAGGER=true
ENABLE_DETAILED_ERRORS=true
LOG_SQL_QUERIES=false
ENABLE_AUTO_MIGRATIONS=false
CORS_ALLOWED_ORIGINS=https://homolog.app.com
```

### Produção

```bash
# Foco: Segurança e performance
ENABLE_SWAGGER=false
ENABLE_DETAILED_ERRORS=false
LOG_SQL_QUERIES=false
ENABLE_AUTO_MIGRATIONS=false
CORS_ALLOWED_ORIGINS=https://app.com
ENABLE_HTTPS_REDIRECT=true
```

## 🔍 Verificar Configuração

### Script de Verificação

```bash
#!/bin/bash
# scripts/verify-env.sh

echo "Verificando variáveis de ambiente..."

# Verificar variáveis obrigatórias
required_vars=(
    "PROPOSTA_DB_CONNECTION_STRING"
    "CONTRATACAO_DB_CONNECTION_STRING"
    "PROPOSTA_SERVICE_BASE_URL"
)

for var in "${required_vars[@]}"; do
    if [ -z "${!var}" ]; then
        echo "❌ Variável $var não definida!"
        exit 1
    else
        echo "✅ $var: definida"
    fi
done

echo "✅ Todas as variáveis obrigatórias estão definidas!"
```

## 📊 Exemplo de Uso no Código

### C# - Ler variável de ambiente

```csharp
// Program.cs
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("PROPOSTA_DB_CONNECTION_STRING")
    ?? throw new InvalidOperationException("Connection string not configured");
```

### appsettings.json com override

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=propostadb;Username=postgres;Password=postgres"
  }
}
```

### Variável de ambiente sobrescreve appsettings

```bash
# Esta variável sobrescreve o appsettings.json
export ConnectionStrings__DefaultConnection="Host=prod;..."
```

## 🐳 Docker Compose com Secrets

```yaml
version: '3.8'

services:
  proposta-api:
    environment:
      - ConnectionStrings__DefaultConnection=${PROPOSTA_DB_CONNECTION_STRING}
    secrets:
      - db_password

secrets:
  db_password:
    external: true
```

## ☸️ Kubernetes ConfigMap e Secrets

```yaml
# ConfigMap (dados não sensíveis)
apiVersion: v1
kind: ConfigMap
metadata:
  name: app-config
data:
  PROPOSTA_SERVICE_BASE_URL: "http://proposta-api"
  LOG_LEVEL: "Warning"

---

# Secret (dados sensíveis)
apiVersion: v1
kind: Secret
metadata:
  name: db-credentials
type: Opaque
stringData:
  PROPOSTA_DB_PASSWORD: "senha_segura"
```

## 🔄 Rotação de Credenciais

### Script de Rotação (Exemplo)

```bash
#!/bin/bash
# scripts/rotate-db-password.sh

NEW_PASSWORD=$(openssl rand -base64 32)

# 1. Atualizar senha no banco
psql -c "ALTER USER app_user WITH PASSWORD '$NEW_PASSWORD';"

# 2. Atualizar no secret manager
aws secretsmanager update-secret \
    --secret-id proposta-db-password \
    --secret-string "$NEW_PASSWORD"

# 3. Reiniciar pods/containers
kubectl rollout restart deployment proposta-api
```

## 📚 Referências

- [12 Factor App - Config](https://12factor.net/config)
- [OWASP - Secure Configuration](https://owasp.org/www-project-top-ten/2017/A6_2017-Security_Misconfiguration)
- [Azure Key Vault](https://azure.microsoft.com/services/key-vault/)
- [AWS Secrets Manager](https://aws.amazon.com/secrets-manager/)
- [HashiCorp Vault](https://www.vaultproject.io/)

## 🆘 Troubleshooting

### Erro: Variável não encontrada

```bash
# Verificar se está definida
echo $PROPOSTA_DB_CONNECTION_STRING

# Definir temporariamente
export PROPOSTA_DB_CONNECTION_STRING="..."

# Definir permanentemente (Linux/Mac)
echo 'export PROPOSTA_DB_CONNECTION_STRING="..."' >> ~/.bashrc
source ~/.bashrc
```

### Erro: Connection string inválida

```bash
# Teste a connection string
psql "Host=localhost;Port=5432;Database=propostadb;Username=postgres"
```

### Docker Compose não lê .env

```bash
# Especificar arquivo explicitamente
docker-compose --env-file .env.development up

# Verificar se o arquivo está no diretório correto
ls -la .env*
```

## ✅ Checklist de Deploy

- [ ] Credenciais atualizadas no secret manager
- [ ] Connection strings com SSL habilitado
- [ ] CORS configurado corretamente
- [ ] Swagger desabilitado (ou com auth)
- [ ] Logs de SQL desabilitados
- [ ] Detailed errors desabilitado
- [ ] HTTPS redirect habilitado
- [ ] Rate limiting configurado
- [ ] Monitoramento configurado
- [ ] Backup configurado

---

**⚠️ IMPORTANTE: Nunca commite credenciais reais no repositório!**


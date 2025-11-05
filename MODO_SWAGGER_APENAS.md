# 📚 Modo Swagger Apenas (Sem Banco de Dados)

Este guia mostra como executar os serviços apenas para visualizar a documentação Swagger, **sem precisar ter o PostgreSQL rodando**.

## 🎯 Executar Apenas para Ver Swagger

Os serviços agora estão configurados para iniciar mesmo sem conexão com o banco de dados. Eles vão mostrar um aviso, mas o Swagger ficará disponível normalmente.

### ContratacaoService

```bash
cd src/ContratacaoService/ContratacaoService.API
dotnet watch run
```

**Acesse**: http://localhost:5002

### PropostaService

```bash
cd src/PropostaService/PropostaService.API
dotnet watch run
```

**Acesse**: http://localhost:5001

## ⚠️ O Que Esperar

Quando o serviço iniciar sem banco de dados, você verá uma mensagem como:

```
⚠️  Warning: Could not connect to database. Running without database.
   Error: Failed to connect to 127.0.0.1:5432
   Swagger is available at the configured URL.
```

Isso é **normal e esperado**! O serviço vai continuar rodando e o Swagger estará disponível.

## ✅ O Que Funcionará

- ✅ **Swagger UI** - Toda a documentação da API
- ✅ **Visualização dos endpoints** - Todos os endpoints documentados
- ✅ **Modelos de dados** - DTOs e schemas disponíveis
- ✅ **Descrições** - Todas as descrições dos endpoints
- ✅ **Try it out** - Você pode testar, mas vai falhar (sem banco)

## ❌ O Que NÃO Funcionará

- ❌ **Execução real das APIs** - Endpoints vão falhar ao tentar acessar banco
- ❌ **Health Check** - `/health` vai reportar erro de banco
- ❌ **Testes funcionais** - Sem dados para retornar

## 🎨 Visualizando a Documentação

### No Swagger UI

1. **Acesse a URL** (http://localhost:5001 ou http://localhost:5002)
2. **Expanda os endpoints** clicando neles
3. **Veja os modelos** na seção "Schemas" no final da página
4. **Leia as descrições** de cada endpoint

### Endpoints Disponíveis

#### PropostaService (http://localhost:5001)

- `GET /api/propostas` - Listar todas as propostas
- `GET /api/propostas/{id}` - Obter proposta específica
- `POST /api/propostas` - Criar nova proposta
- `PATCH /api/propostas/{id}/status` - Alterar status da proposta

#### ContratacaoService (http://localhost:5002)

- `GET /api/contratacoes` - Listar todas as contratações
- `GET /api/contratacoes/{id}` - Obter contratação específica
- `POST /api/contratacoes` - Contratar uma proposta

## 💡 Dicas

1. **Use o Swagger para entender a estrutura** dos dados
2. **Copie os exemplos** de request/response
3. **Documente seus testes** baseado na API
4. **Quando precisar testar de verdade**, inicie o PostgreSQL

## 🗄️ Quando Quiser Usar o Banco

Para testar as APIs de verdade, inicie o PostgreSQL:

### Opção 1: Docker (Recomendado)

```bash
docker run --name postgres-local \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:16
```

### Opção 2: Docker Compose (Tudo junto)

```bash
docker-compose up -d
```

Depois reinicie o serviço com `Ctrl+C` e execute novamente:

```bash
dotnet watch run
```

Agora você verá:

```
✅ Database migration completed successfully.
```

## 🔄 Alternando Entre Modos

### Modo Apenas Swagger (Sem Banco)
- Execute `dotnet watch run` sem PostgreSQL rodando
- Swagger fica disponível
- APIs não funcionam

### Modo Completo (Com Banco)
- Inicie o PostgreSQL
- Execute `dotnet watch run`
- Tudo funciona completamente

## 📖 Documentação Relacionada

- [EXECUTAR_SERVICOS.md](EXECUTAR_SERVICOS.md) - Guia completo de execução
- [QUICK_START.md](QUICK_START.md) - Guia de início rápido
- [README.md](README.md) - Documentação completa

---

**Aproveite o Swagger! 📚**


-- Script de inicialização do banco de dados ContratacaoService
-- PostgreSQL

-- Criar tabela de contratações
CREATE TABLE IF NOT EXISTS "Contratacoes" (
    "Id" UUID PRIMARY KEY,
    "PropostaId" UUID NOT NULL,
    "NumeroApolice" VARCHAR(50) NOT NULL,
    "DataContratacao" TIMESTAMP NOT NULL,
    "DataVigenciaInicio" TIMESTAMP NOT NULL,
    "DataVigenciaFim" TIMESTAMP NOT NULL,
    "ValorPremio" DECIMAL(18,2) NOT NULL
);

-- Criar índices únicos
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Contratacoes_PropostaId" ON "Contratacoes" ("PropostaId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Contratacoes_NumeroApolice" ON "Contratacoes" ("NumeroApolice");
CREATE INDEX IF NOT EXISTS "IX_Contratacoes_DataContratacao" ON "Contratacoes" ("DataContratacao");

-- Comentários nas tabelas
COMMENT ON TABLE "Contratacoes" IS 'Tabela de contratações de seguro';
COMMENT ON COLUMN "Contratacoes"."PropostaId" IS 'Referência à proposta aprovada no PropostaService';


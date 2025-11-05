-- Script de inicialização do banco de dados PropostaService
-- PostgreSQL

-- Criar tabela de propostas
CREATE TABLE IF NOT EXISTS "Propostas" (
    "Id" UUID PRIMARY KEY,
    "NomeCliente" VARCHAR(200) NOT NULL,
    "CpfCliente" VARCHAR(11) NOT NULL,
    "TipoSeguro" VARCHAR(50) NOT NULL,
    "ValorCobertura" DECIMAL(18,2) NOT NULL,
    "ValorPremio" DECIMAL(18,2) NOT NULL,
    "Status" INTEGER NOT NULL,
    "DataCriacao" TIMESTAMP NOT NULL,
    "DataAtualizacao" TIMESTAMP NULL
);

-- Criar índices
CREATE INDEX IF NOT EXISTS "IX_Propostas_Status" ON "Propostas" ("Status");
CREATE INDEX IF NOT EXISTS "IX_Propostas_CpfCliente" ON "Propostas" ("CpfCliente");
CREATE INDEX IF NOT EXISTS "IX_Propostas_DataCriacao" ON "Propostas" ("DataCriacao");

-- Inserir dados de exemplo
INSERT INTO "Propostas" ("Id", "NomeCliente", "CpfCliente", "TipoSeguro", "ValorCobertura", "ValorPremio", "Status", "DataCriacao", "DataAtualizacao")
VALUES 
    (gen_random_uuid(), 'João da Silva', '12345678909', 'Vida', 100000.00, 500.00, 1, NOW(), NULL),
    (gen_random_uuid(), 'Maria Santos', '98765432100', 'Auto', 50000.00, 300.00, 2, NOW(), NULL),
    (gen_random_uuid(), 'Pedro Oliveira', '45678912345', 'Residencial', 200000.00, 800.00, 1, NOW(), NULL);

-- Comentários nas tabelas
COMMENT ON TABLE "Propostas" IS 'Tabela de propostas de seguro';
COMMENT ON COLUMN "Propostas"."Status" IS '1=EmAnalise, 2=Aprovada, 3=Rejeitada';


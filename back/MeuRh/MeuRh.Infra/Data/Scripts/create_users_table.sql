-- Script SQL para criação da tabela Users
-- Baseado no mapeamento Entity Framework Core (UserMap.cs)
-- Banco de dados: PostgreSQL

CREATE TABLE IF NOT EXISTS "Users" (
    "Id" UUID NOT NULL,
    "Name" VARCHAR(200) NOT NULL,
    "Email" VARCHAR(320) NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");

-- Comentários sobre as colunas:
-- Id: GUID/UUID - Chave primária gerada automaticamente
-- Name: Nome do usuário (máximo 200 caracteres)
-- Email: E-mail do usuário (máximo 320 caracteres, único)
-- PasswordHash: Hash da senha do usuário
-- CreatedAt: Data e hora de criação do registro (UTC)
-- IsActive: Indica se o usuário está ativo (padrão: true)


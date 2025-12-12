-- Script SQL para criação da tabela Users (SQL Server)
-- Baseado no mapeamento Entity Framework Core (UserMap.cs)
-- Banco de dados: SQL Server

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [Email] NVARCHAR(320) NOT NULL,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
END

-- Comentários sobre as colunas:
-- Id: UNIQUEIDENTIFIER - Chave primária gerada automaticamente
-- Name: Nome do usuário (máximo 200 caracteres)
-- Email: E-mail do usuário (máximo 320 caracteres, único)
-- PasswordHash: Hash da senha do usuário
-- CreatedAt: Data e hora de criação do registro (UTC)
-- IsActive: Indica se o usuário está ativo (padrão: 1/true)


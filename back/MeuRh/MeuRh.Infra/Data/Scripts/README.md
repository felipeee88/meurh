# Scripts SQL - Mapeamento do Banco de Dados

Este diretório contém os scripts SQL baseados no mapeamento Entity Framework Core do projeto.

## Estrutura da Tabela Users

A tabela `Users` é mapeada a partir da entidade `User` (MeuRh.Domain.Entities.User) e configurada através de `UserMap` (MeuRh.Infra.Data.Configurations.UserMap).

### Mapeamento

| Propriedade C# | Tipo C# | Coluna SQL (PostgreSQL) | Tipo SQL (PostgreSQL) | Restrições |
|----------------|---------|-------------------------|----------------------|------------|
| Id | Guid | Id | UUID | PRIMARY KEY, NOT NULL |
| Name | string | Name | VARCHAR(200) | NOT NULL, MaxLength(200) |
| Email | string | Email | VARCHAR(320) | NOT NULL, MaxLength(320), UNIQUE INDEX |
| PasswordHash | string | PasswordHash | TEXT | NOT NULL |
| CreatedAt | DateTime | CreatedAt | TIMESTAMP | NOT NULL |
| IsActive | bool | IsActive | BOOLEAN | NOT NULL, DEFAULT true |

### Índices

- **PK_Users**: Chave primária na coluna `Id`
- **IX_Users_Email**: Índice único na coluna `Email`

## Script Disponível

- `create_users_table.sql` - Script para PostgreSQL

## Como Usar

```bash
psql -U postgres -d MeuRhDb -f create_users_table.sql
```

Ou execute diretamente no PostgreSQL:

```sql
\i create_users_table.sql
```

## Nota Importante

O projeto utiliza **Database First** (não Code First). As tabelas devem ser criadas manualmente executando os scripts SQL antes de iniciar a aplicação.


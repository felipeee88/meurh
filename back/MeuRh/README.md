# MeuRh Backend API

Backend .NET 8 com Clean Architecture + DDD + CQRS + JWT + Swagger + Testes

## Estrutura do Projeto

A solução está organizada em camadas seguindo os princípios de Clean Architecture:

- **MeuRh.Domain**: Entidades de domínio, interfaces de repositório e exceções de domínio
- **MeuRh.Application**: DTOs, Commands/Queries (CQRS), Handlers, Validators e interfaces de serviços
- **MeuRh.Infra**: Implementações de repositório, EF Core, serviços (PasswordHasher, TokenService) e configurações
- **MeuRh.Api**: Controllers, middleware, configuração de autenticação JWT e Swagger
- **MeuRh.Tests**: Testes automatizados com xUnit, Moq e FluentAssertions

## Tecnologias Utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8.0
- PostgreSQL (Npgsql)
- MediatR (CQRS)
- FluentValidation
- JWT Bearer Authentication
- Swagger/OpenAPI
- xUnit, Moq, FluentAssertions

## Pré-requisitos

- .NET 8 SDK
- PostgreSQL (ou altere a connection string para SQL Server)
- Visual Studio 2022 ou VS Code

## Configuração

### 1. Banco de Dados

Configure a connection string no arquivo `appsettings.json` ou `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=MeuRhDb;Username=postgres;Password=postgres"
  }
}
```

### 2. JWT Configuration

Configure as chaves JWT no `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!",
    "Issuer": "MeuRh",
    "Audience": "MeuRh",
    "ExpirationMinutes": "60"
  }
}
```

**Importante**: Em produção, use uma chave secreta forte e armazene-a de forma segura (variáveis de ambiente, Azure Key Vault, etc.).

## Executando o Projeto

### 1. Restaurar dependências

```bash
dotnet restore
```

### 2. Executar a aplicação

```bash
cd MeuRh.Api
dotnet run
```

A aplicação estará disponível em:
- HTTP: `http://localhost:5222`
- HTTPS: `https://localhost:7013`
- Swagger: `http://localhost:5222` ou `https://localhost:7013`

### 3. Executar testes

```bash
dotnet test
```

## Endpoints da API

### Autenticação

#### POST /api/auth/login
Autentica um usuário e retorna um token JWT.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

### Usuários

#### POST /api/users
Cria um novo usuário (público).

**Request:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "John Doe",
  "email": "john@example.com",
  "createdAt": "2024-01-01T00:00:00Z"
}
```

#### GET /api/users
Lista todos os usuários (requer autenticação).

**Headers:**
```
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "John Doe",
    "email": "john@example.com",
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

## Usando o Swagger

1. Acesse `http://localhost:5222` ou `https://localhost:7013`
2. Para testar endpoints protegidos:
   - Primeiro, faça login em `/api/auth/login`
   - Copie o `accessToken` da resposta
   - Clique no botão "Authorize" no topo do Swagger
   - Cole o token no formato: `Bearer {seu_token}`
   - Agora você pode testar os endpoints protegidos

## Testes

Os testes estão localizados em `MeuRh.Tests` e incluem:

- **CreateUserCommandHandlerTests**: Testa a criação de usuários
  - Cenário de sucesso (email não existe)
  - Cenário de erro (email duplicado)

- **LoginCommandHandlerTests**: Testa a autenticação
  - Cenário de sucesso (credenciais válidas)
  - Cenário de erro (usuário não existe)
  - Cenário de erro (senha inválida)

Execute os testes com:
```bash
dotnet test
```

## Arquitetura

### Clean Architecture

A aplicação segue os princípios de Clean Architecture:

- **Domain**: Não depende de nenhuma outra camada
- **Application**: Depende apenas do Domain
- **Infrastructure**: Depende de Domain e Application
- **API**: Depende de todas as camadas

### CQRS

- **Commands**: Operações de escrita (CreateUser, Login)
- **Queries**: Operações de leitura (GetUsers)
- **Handlers**: Processam Commands e Queries

### DDD

- **Entidades**: User (com regras de negócio)
- **Repositórios**: IUserRepository (abstração de persistência)
- **Exceções de Domínio**: BusinessRuleException, DomainException

## Validações

As validações são feitas com FluentValidation:

- **CreateUserCommandValidator**: Valida nome, email e senha
- **LoginCommandValidator**: Valida email e senha

As validações são executadas automaticamente via pipeline do MediatR antes dos handlers.

## Segurança

- Senhas são armazenadas como hash (PBKDF2)
- Autenticação JWT com expiração configurável
- Endpoints protegidos com `[Authorize]`
- Validação de entrada em todos os endpoints

## Próximos Passos

- [ ] Adicionar refresh tokens
- [ ] Implementar rate limiting
- [ ] Adicionar logging estruturado (Serilog)
- [ ] Implementar health checks
- [ ] Adicionar migrations do EF Core
- [ ] Configurar CI/CD
- [ ] Adicionar mais testes de integração


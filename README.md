Teste Prático — Desenvolvedor Fullstack
Objetivo

Desenvolver uma aplicação fullstack mínima, composta por:

API Backend (.NET Core 8)

Frontend (Angular 19)

A aplicação deve permitir:

Cadastro de usuários

Listagem dos usuários cadastrados

Autenticação de usuários

Com foco em arquitetura, segurança e boas práticas de desenvolvimento.

Requisitos Obrigatórios
Backend (.NET Core 8)
Endpoints da API

POST /users

Cadastra um novo usuário

Campos obrigatórios:

Nome

E-mail

Regras:

O e-mail deve ser único

GET /users

Retorna a lista de usuários cadastrados

Persistência de Dados

Utilizar banco de dados relacional

Utilizar Entity Framework Core (EF Core) como ORM

Arquitetura

Seguir os princípios de Clean Architecture

Código:

Manutenível

Extensível

Utilização do padrão CQRS (Command Query Responsibility Segregation)

Segurança

Implementar autenticação utilizando JWT (JSON Web Token)

Frontend (Angular 19)
Funcionalidades

Tela de cadastro de usuário

Tela de listagem de usuários

Boas Práticas

Modularização por funcionalidade

Exemplo: UsersModule

Uso de HTTP Interceptor para envio do token JWT no header das requisições

Uso de Route Guard para proteção das rotas autenticadas (pode ser simples)

Testes Automatizados

Implementar ao menos 1 teste automatizado, podendo ser:

Teste de handler no backend, ou

Teste de componente no frontend

Deploy / Execução

Entregar uma das opções abaixo:

Um Dockerfile funcional, ou

Um README.md com instruções claras de build e execução, por exemplo:

Uso de docker-compose

Comandos para subir backend e frontend

Restrições

O foco está na clareza da arquitetura, organização e boas práticas

Não é necessário desenvolver interface visual sofisticada no frontend

O código deve ser entregue em um repositório Git:

GitHub

GitLab

Ou link direto

Tempo Estimado

O teste foi desenhado para ser concluído em até 6 horas de dedicação

Critérios de Avaliação

Serão avaliados:

Organização do projeto e arquitetura

Boas práticas no backend (CQRS, validação e segurança)

Boas práticas no frontend (modularização e consumo seguro da API)

Qualidade dos testes entregues

Capacidade de fornecer um ambiente pronto para execução

Entrega

Enviar o link do repositório Git até a data combinada

---

## Executando o Projeto

### Backend (.NET Core 8)

1. Navegue até a pasta do backend:
```bash
cd back/MeuRh/MeuRh.Api
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Execute o projeto:
```bash
dotnet run
```

O backend estará disponível em:
- **HTTP:** `http://localhost:5222`
- **HTTPS:** `https://localhost:7013`
- **Swagger/OpenAPI:** `http://localhost:5222` ou `https://localhost:7013`

### Frontend (Angular 19)

1. Navegue até a pasta do frontend:
```bash
cd front
```

2. Instale as dependências:
```bash
npm install
```

3. Execute o projeto:
```bash
npm start
```

O frontend estará disponível em:
- **URL:** `http://localhost:4200`

### Configuração

O frontend está configurado para consumir a API do backend através da URL base:
- **API Base URL:** `http://localhost:5222`

A configuração está definida nos arquivos de environment:
- `front/src/environments/environment.ts`
- `front/src/environments/environment.development.ts`

### Endpoints da API

#### Autenticação

**POST** `/api/auth/login`
- Autentica um usuário e retorna um token JWT
- Request body:
  ```json
  {
    "email": "user@example.com",
    "password": "password123"
  }
  ```
- Response:
  ```json
  {
    "status": "Sucesso",
    "message": "Login realizado com sucesso",
    "data": {
      "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "tokenType": "Bearer",
      "expiresIn": 3600
    }
  }
  ```

#### Usuários

**POST** `/api/users`
- Cria um novo usuário (público, não requer autenticação)
- Request body:
  ```json
  {
    "name": "John Doe",
    "email": "john@example.com",
    "password": "password123"
  }
  ```

**GET** `/api/users?name={nome}` (opcional)
- Lista todos os usuários (requer autenticação)
- Query parameter opcional: `name` para filtrar por nome
- Headers:
  ```
  Authorization: Bearer {token}
  ```

**DELETE** `/api/users/{id}`
- Exclui um usuário (requer autenticação)
- Headers:
  ```
  Authorization: Bearer {token}
  ```

### CORS

O backend está configurado para permitir requisições do frontend em `http://localhost:4200`.

Para mais detalhes sobre a API, consulte a documentação Swagger em `http://localhost:5222` quando o backend estiver em execução.
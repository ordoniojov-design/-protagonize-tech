# TaskBoard – Desafio Técnico Avanade

Aplicação de gerenciamento de tarefas estilo Kanban (inspirado no Trello/Jira), desenvolvida com Angular no front-end e ASP.NET Core 9 Web API no back-end.

## Tecnologias

- Front-end: Angular 17 (Standalone Components)
- Back-end: ASP.NET Core 9 Web API (C#)
- Banco de dados: SQL Server
- ORM: Entity Framework Core 9
- Comunicação: REST API (JSON)
- Testes: xUnit + EF Core InMemory
- Containerização: Docker + Docker Compose

## Funcionalidades

- Board Kanban com 3 colunas: Pendente, Em Andamento, Concluída
- Criar, editar e excluir tarefas
- Mover tarefas entre colunas com um clique
- Prioridade por tarefa (Alta / Média / Baixa) com indicação visual
- Filtro por prioridade
- Validação de formulário
- Mensagens de sucesso/erro (toast)
- Contador de progresso no header

---

## Como rodar o projeto

### Opção 1 — Docker (recomendado)

Pré-requisito: [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado.

```bash
docker-compose up --build
```

Isso sobe três containers:
- `sqlserver` — SQL Server 2022 Express na porta 1433
- `api` — ASP.NET Core 9 API na porta 5000 (migrations aplicadas automaticamente na inicialização)
- `frontend` — Angular servido via nginx na porta 4200

Acesse em `http://localhost:4200`
Swagger UI: `http://localhost:5000/swagger`

Para derrubar tudo:
```bash
docker-compose down
```

Para derrubar e apagar o volume do banco:
```bash
docker-compose down -v
```

---

### Opção 2 — Execução local

#### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) e npm
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`
- SQL Server (local ou Docker)

#### 1. Back-end

```bash
cd backend/TaskManager.API
dotnet restore
dotnet ef database update
dotnet run
```

A API estará disponível em `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

> Para ajustar a connection string, edite `appsettings.json`:
> ```json
> "DefaultConnection": "Server=localhost;Database=TaskManagerDB;Trusted_Connection=True;TrustServerCertificate=True;"
> ```

#### 2. Front-end

```bash
cd frontend
npm install
ng serve
```

Acesse em `http://localhost:4200`

---

## Testes

```bash
dotnet test backend/TaskManager.Tests/TaskManager.Tests.csproj
```

Os testes usam EF Core InMemory — sem necessidade de banco de dados. 28 testes no total.

| Classe | Cobertura |
|--------|-----------|
| `TarefaServiceTests` | GetAll, GetById, Create, Update, UpdateStatus, Delete |
| `TarefasControllerTests` | Status codes HTTP de cada endpoint |
| `ServiceResultTests` | Ok, Created, NotFound, BadRequest, InternalError |

---

## Endpoints da API

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/tarefas` | Listar todas (aceita `?status=Pendente`) |
| GET | `/api/tarefas/{id}` | Buscar por ID |
| POST | `/api/tarefas` | Criar tarefa |
| PUT | `/api/tarefas/{id}` | Atualizar tarefa |
| PATCH | `/api/tarefas/{id}/status` | Atualizar apenas o status |
| DELETE | `/api/tarefas/{id}` | Excluir tarefa |

---

## Estrutura do projeto

```
├── .gitignore
├── docker-compose.yml
│
├── backend/
│   ├── TaskManager.API/
│   │   ├── Controllers/       # Endpoints REST da API
│   │   ├── Data/              # DbContext e Migrations do EF Core
│   │   ├── DTOs/              # Objetos de transferência de dados (entrada e saída)
│   │   ├── Models/            # Entidades do banco de dados
│   │   ├── Services/          # Lógica de negócio e wrapper de resultado
│   │   ├── appsettings.json
│   │   ├── appsettings.Docker.json
│   │   ├── Dockerfile
│   │   └── Program.cs
│   │
│   └── TaskManager.Tests/
│       ├── Controllers/       # Testes dos endpoints e status codes HTTP
│       ├── Helpers/           # Utilitários compartilhados entre os testes
│       └── Services/          # Testes da lógica de negócio e do ServiceResult
│
└── frontend/
    └── src/app/
        ├── components/
        │   ├── kanban-board/  # Board principal com as colunas Kanban
        │   ├── task-card/     # Card individual de cada tarefa
        │   └── task-modal/    # Modal de criação e edição de tarefas
        ├── models/            # Interfaces e tipos TypeScript
        └── services/          # Comunicação com a API via HttpClient
```

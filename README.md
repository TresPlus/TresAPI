# TresAPI

A multi-database backend API platform designed for modern, scalable, and modular applications.

**TresAPI** provides first-class support for MySQL, PostgreSQL, MSSQL, and MongoDB, built on a clean architecture with domain-driven principles, a fully custom migration engine, and provider-based persistence infrastructure. It is specifically designed to serve as a backend for managing 3D assets in the cloud with JWT authentication.

---

## 🚀 Features

*   **Database-Agnostic Architecture:** Custom abstraction layer supporting multiple DB types.
*   **Multi-Provider Support:** MySQL, PostgreSQL, MSSQL, and MongoDB (In Progress).
*   **Custom Cross-Database Migration Engine:** Works consistently across all supported providers without relying on EF Core migrations strictly.
*   **Clean Architecture + DDD:** Strong separation of concerns & testability.
*   **Cross-Provider Persistence Layer:** Independent database providers under a unified interface.
*   **Scalable REST API with .NET:** High-performance minimal overhead.
*   **Modular and Extensible:** Plug-and-play providers and modules.
*   **OpenAPI / Swagger Integration:** Easy API documentation and testing.

---

## 📁 Project Structure

```text
TresAPI (Solution)
├── API
│   ├── wwwroot (Assets: .FBX, .OBJ, .PNG files...)
│   ├── Controllers
│   ├── DTOs (Data Transfer Objects)
│   ├── Extensions
├── Business
│   ├── Abstract (Interfaces)
│   ├── Concrete (Implementations)
│   ├── Static (Defaults, JSON Types...)
├── DataAccess
│   ├── Abstract (Repository Interfaces)
│   ├── Concrete (Repository Implementations)
│   ├── Interactive (DbContexts...)
│   ├── Migrations (Schema Creators)
├── Entities (Domain Models)
└── ResultLayer
    ├── Abstract (Result Interfaces)
    ├── Concrete (Result Implementations)
```

---

## 🗄 Database Providers

TresAPI supports multiple databases through a provider-based architecture:

| Provider | Status | Notes |
| :--- | :--- | :--- |
| **MySQL** | ✔️ Ready | Supports full migration engine |
| **PostgreSQL** | ✔️ Ready | Optimized query generation |
| **MSSQL** | ✔️ Ready | Native identity + GUID support |
| **MongoDB** | 🚧 In Dev | Document-based migrations included |

Each provider has its own configuration module and custom migration handlers.

---

## 🔄 Custom Migration Engine

TresAPI includes a fully custom migration engine designed to work across relational and NoSQL databases.

### Capabilities
*   Create, update, and delete schemas/tables/collections.
*   Version-controlled migrations.
*   Provider-specific execution pipelines.
*   Database-safe up/down operations.

### Example Migration Definition

```csharp
public class CreateUsersTable : Migration
{
    public override void Up()
    {
        Table("Users")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Email").AsString().NotNullable()
            .WithColumn("CreatedAt").AsDateTime();
    }

    public override void Down()
    {
        DropTable("Users");
    }
}
```

---

## 🧱 Architecture Overview

TresAPI follows **Clean Architecture** with domain-focused design.

*   **API:** HTTP layer with validation & middleware.
*   **Business:** Value objects, core business rules, validation.
*   **DataAccess:** Database providers, repositories & migrations.
*   **Entities:** Core domain entities.
*   **Result Layer:** Standardized Success/Failed result wrapper system.

---

## 📦 Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/TresPlus/TresAPI.git
```

### 2. Navigate to the project
```bash
cd TresAPI/src/Tres.API
```

### 3. Configure the environment
Create your own `appsettings.Development.json` file:

```json
{
  "Database": {
    "Provider": "MySQL",
    "ConnectionString": "Server=localhost;Database=TresDB;User=root;Password=yourpassword;"
  },
  "Environment": {
  "name": "...",
  "Issuer": "..."

},
"AuthProviders": {
  "OAuthProviders": {
    "GitHub": {
      "ClientId": "...",
      "ClientSecret": "...",
      "AuthorizationEndpoint": "...",
      "TokenEndpoint": "...",
      "UserInformationEndpoint": "...",
      "CallbackPath": "...",
      "Scopes": [ "user:email" ]
    }
  },
  "OpenIdConnectProviders": {
    "Google": {
      "ClientId": "...",
      "ClientSecret": "...",
      "Authority": "https://accounts.google.com",
      "CallbackPath": "/signin-google",
      "Scopes": [ "openid", "profile", "email" ]
    },
    "Microsoft": {
      "ClientId": "...",
      "ClientSecret": "...",
      "Authority": "https://login.microsoftonline.com/common",
      "CallbackPath": "/signin-microsoft",
      "Scopes": [ "openid", "profile", "email" ]
    }
  }
},
"SMTP": {
  "Host": "...",
  "Port": "...",
  "Username": "",
  "Password": "...",
  "From": "no-reply@...",
  "SSL": true
},

"Jwt": {
  "Key": "...",
  "Issuer": "...",
  "Audience": "...",
  "Algorithm": "HS256"
}
}
```

### 4. Run the API
```bash
dotnet run
```

---

## 🧰 Useful Commands

**Run API:**
```bash
dotnet run --project src/Tres.API
```

**Run Tests:**
```bash
dotnet test
```

**Add Migration:**
```bash
dotnet run --project src/TresAPI -- add-migration "MigrationName"
```

**Apply Migrations:**
```bash
dotnet run --project src/TresAPI -- migrate
```

---

## compass: Roadmap

- [x] MySQL, PostgreSQL, MSSQL Providers
- [ ] MongoDB provider full release
- [ ] Database-agnostic LINQ provider
- [ ] Distributed caching support (Redis)
- [ ] Authentication Module (JWT & OAuth2)
- [ ] CLI tool for migrations
- [ ] Dockerized orchestration environment
- [ ] WebSocket real-time extension
- [ ] Telemetry + Observability module

---

## 📄 License

[MIT License](LICENSE) — open source, free to use.
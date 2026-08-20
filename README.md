# Prueba técnica CLT

Web API construida con ASP.NET Core Minimal API, .NET 10, CQRS simple, EF Core con SQLite y FluentValidation.

## Ejecución

```powershell
dotnet restore
dotnet ef database update
dotnet run
```

La API usa `http://localhost:5102`. En Development, Swagger UI está disponible en `http://localhost:5102/swagger`.

Todos los endpoints de negocio requieren este header:

```text
X-API-KEY: clt-development-api-key
```

La clave se lee desde `Security:ApiKey` en `appsettings.json` y puede reemplazarse mediante la variable de entorno `Security__ApiKey`.

## Endpoints

- `POST /users`
- `GET /users?isActive=true|false`
- `GET /users/{id}`
- `PUT /users/{id}`
- `DELETE /users/{id}`
- `POST /users/{userId}/addresses`
- `GET /users/{userId}/addresses`
- `PUT /addresses/{id}`
- `DELETE /addresses/{id}`
- `GET /currencies`
- `POST /currencies`
- `POST /currency/convert`

Los passwords se almacenan usando `PasswordHasher<TUser>`; nunca se devuelven en las respuestas. Email y código de moneda tienen índices únicos con comparación case-insensitive en SQLite.

# Prueba técnica CLT

Web API construida con ASP.NET Core Minimal API, .NET 10, CQRS simple, EF Core con SQLite y FluentValidation.

## Ejecución

```powershell
dotnet restore
dotnet tool install --global dotnet-ef
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
- `GET /users?name={name}&email={email}&isActive=true|false`
- `GET /users/{id}`
- `PUT /users/{id}`
- `PATCH /users/{id}`
- `DELETE /users/{id}`
- `POST /users/{userId}/addresses`
- `GET /users/{userId}/addresses?id={id}&street={street}&city={city}&country={country}&zipCode={zipCode}`
- `GET /addresses`
- `GET /addresses/{id}`
- `PUT /addresses/{id}`
- `PATCH /addresses/{id}`
- `DELETE /addresses/{id}`
- `GET /currencies?code={code}&name={name}`
- `GET /currencies/{id}`
- `POST /currencies`
- `POST /currency/convert`

Los endpoints rechazan con `400 Bad Request` cualquier parámetro de consulta no reconocido. Los filtros opcionales tampoco pueden enviarse vacíos.

Los passwords se almacenan usando `PasswordHasher<TUser>`; nunca se devuelven en las respuestas. Email y código de moneda tienen índices únicos con comparación case-insensitive en SQLite.

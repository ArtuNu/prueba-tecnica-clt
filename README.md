# Prueba técnica CLT

API REST para administrar usuarios, direcciones y monedas, y realizar conversiones monetarias. Está construida con ASP.NET Core Minimal APIs sobre .NET 10, Entity Framework Core con SQLite, FluentValidation y una separación simple de comandos y consultas (CQRS).

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- EF Core CLI sólo si se van a crear o aplicar migraciones manualmente:

```powershell
dotnet tool install --global dotnet-ef --version "10.*"
```

Si la herramienta ya está instalada, se puede comprobar con `dotnet ef --version`.

## Instalación y primera ejecución

Desde la raíz del repositorio:

```powershell
dotnet restore
dotnet ef database update
dotnet run
```

El perfil predeterminado inicia la API en `http://localhost:5102` con el entorno `Development`. La base SQLite se crea o actualiza en `prueba-tecnica-clt.db` al ejecutar las migraciones.

Para usar el perfil HTTPS:

```powershell
dotnet run --launch-profile https
```

Sus URLs son `https://localhost:7208` y `http://localhost:5102`. Si el certificado local todavía no es confiable, se puede registrar con `dotnet dev-certs https --trust`.

## Swagger / OpenAPI

Con la aplicación ejecutándose en `Development`:

- Swagger UI: `http://localhost:5102/swagger`
- Documento OpenAPI: `http://localhost:5102/openapi/v1.json`

En Swagger UI, pulse **Authorize**, introduzca la API key y luego pruebe cualquier operación. Swagger y el documento OpenAPI son públicos; los endpoints de negocio requieren autenticación.

Estas rutas sólo se exponen en `Development`. Para habilitarlas con otra configuración, establezca `ASPNETCORE_ENVIRONMENT=Development` antes de iniciar la aplicación.

## Autenticación y configuración

Todas las solicitudes de negocio deben incluir:

```http
X-API-KEY: clt-development-api-key
```

La clave y la conexión se configuran en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=prueba-tecnica-clt.db"
  },
  "Security": {
    "ApiKey": "clt-development-api-key"
  }
}
```

Para no modificar el archivo, se pueden usar variables de entorno. En PowerShell:

```powershell
$env:Security__ApiKey = "otra-clave-segura"
$env:ConnectionStrings__DefaultConnection = "Data Source=C:\data\clt.db"
dotnet run
```

La clave incluida es únicamente para desarrollo. Una clave ausente o incorrecta devuelve `401 Unauthorized`.

## Probar la API con el archivo `.http`

[`prueba-tecnica-clt.http`](prueba-tecnica-clt.http) contiene ejemplos para todo el flujo: usuarios, direcciones, monedas y conversión. Puede ejecutarse desde Visual Studio, Rider o VS Code con una extensión compatible con archivos HTTP.

1. Inicie la API con `dotnet run`.
2. Ajuste `host`, `apiKey` y los identificadores al comienzo del archivo si fuese necesario.
3. Ejecute las solicitudes de arriba hacia abajo; las consultas por ID asumen que los recursos ya existen.

## Endpoints

| Método | Ruta | Uso |
| --- | --- | --- |
| `POST` | `/users` | Crear un usuario. |
| `GET` | `/users` | Listar y filtrar por `name`, `email` o `isActive`. |
| `GET` | `/users/{id}` | Obtener un usuario. |
| `PUT` | `/users/{id}` | Reemplazar los datos editables del usuario. |
| `PATCH` | `/users/{id}` | Modificar uno o más campos del usuario. |
| `DELETE` | `/users/{id}` | Eliminar un usuario y sus direcciones. |
| `POST` | `/users/{userId}/addresses` | Agregar una dirección al usuario. |
| `GET` | `/users/{userId}/addresses` | Listar sus direcciones y filtrar por `addressId`, `street`, `city`, `country` o `zipCode`. |
| `GET` | `/addresses` | Listar todas las direcciones. |
| `GET` | `/addresses/{id}` | Obtener una dirección. |
| `PUT` | `/addresses/{id}` | Reemplazar una dirección. |
| `PATCH` | `/addresses/{id}` | Modificar uno o más campos de una dirección. |
| `DELETE` | `/addresses/{id}` | Eliminar una dirección. |
| `GET` | `/currencies` | Listar y filtrar por `code` o `name`. |
| `GET` | `/currencies/{id}` | Obtener una moneda. |
| `POST` | `/currencies` | Crear una moneda. |
| `POST` | `/currency/convert` | Convertir un importe entre dos monedas registradas. |

### Reglas principales

- `email` y `code` son únicos sin distinguir mayúsculas de minúsculas. Los emails se guardan en minúsculas y los códigos de moneda en mayúsculas.
- El password debe tener entre 8 y 100 caracteres. Se almacena como hash y nunca aparece en las respuestas.
- Un usuario nuevo queda activo. `PUT /users/{id}` requiere `name`, `email` e `isActive`; `password` es opcional y, si se omite, conserva el actual.
- En un `PATCH` debe enviarse al menos un campo. Para quitar el código postal se puede enviar `"zipCode": ""`.
- Cada usuario puede tener varias direcciones. Al eliminarlo, sus direcciones se eliminan en cascada.
- `rateToBase` debe ser mayor que cero y expresar cuántas unidades de la moneda base equivale una unidad de esa moneda. La conversión aplicada es `amount × from.rateToBase ÷ to.rateToBase`.
- Los filtros de texto buscan coincidencias parciales. `isActive` sólo acepta `true` o `false`.
- Los parámetros de consulta desconocidos producen `400 Bad Request`; tampoco se admiten filtros vacíos.

## Respuestas y errores

- `200 OK`: consulta, actualización o conversión exitosa.
- `201 Created`: recurso creado; incluye el header `Location`.
- `204 No Content`: eliminación exitosa.
- `400 Bad Request`: JSON mal formado, validación fallida o query string inválida.
- `401 Unauthorized`: API key ausente o incorrecta.
- `404 Not Found`: recurso o ruta inexistente.
- `409 Conflict`: email o código de moneda duplicado.

Los errores de validación usan el formato estándar `HttpValidationProblemDetails`; los conflictos y varios errores de dominio usan `{ "error": "mensaje" }`.

## Comandos útiles

```powershell
# Compilar
dotnet build

# Aplicar migraciones pendientes
dotnet ef database update

# Crear una migración después de cambiar el modelo
dotnet ef migrations add NombreDeLaMigracion

# Ejecutar sin usar launchSettings.json
dotnet run --no-launch-profile --urls http://localhost:5102
```

## Estructura

- `Domain/`: entidades del dominio.
- `Application/`: DTOs, comandos, consultas, validadores y handlers.
- `Infrastructure/`: contexto y configuración de persistencia.
- `Endpoints/`: rutas Minimal API y adaptación HTTP.
- `Middleware/`: autenticación por API key.
- `Migrations/`: historial de esquema de EF Core.

using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PruebaTecnicaClt.Application.Addresses.Commands;
using PruebaTecnicaClt.Application.Addresses.Queries;
using PruebaTecnicaClt.Application.CurrencyConversion;
using PruebaTecnicaClt.Application.Currencies.Commands;
using PruebaTecnicaClt.Application.Currencies.Queries;
using PruebaTecnicaClt.Application.Users.Commands;
using PruebaTecnicaClt.Application.Users.Queries;
using PruebaTecnicaClt.Domain.Entities;
using PruebaTecnicaClt.Endpoints;
using PruebaTecnicaClt.Infrastructure.Persistence;
using PruebaTecnicaClt.Middleware;
using System.Globalization;

var culture = new CultureInfo("es-ES");

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
ValidatorOptions.Global.LanguageManager.Culture = culture;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Prueba Tecnica CLT API";
        document.Info.Version = "v1";
        document.Info.Description =
            "API REST para administrar usuarios, direcciones y monedas, y realizar conversiones monetarias. " +
            "Todos los endpoints de negocio requieren el header X-API-KEY.";
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["ApiKey"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "X-API-KEY",
            In = ParameterLocation.Header,
            Description = "Clave requerida por todos los endpoints de negocio."
        };
        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("ApiKey", document, null)] = []
        });

        return Task.CompletedTask;
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<CreateUserCommandHandler>();
builder.Services.AddScoped<UpdateUserCommandHandler>();
builder.Services.AddScoped<PatchUserCommandHandler>();
builder.Services.AddScoped<DeleteUserCommandHandler>();
builder.Services.AddScoped<GetUsersQueryHandler>();
builder.Services.AddScoped<GetUserByIdQueryHandler>();
builder.Services.AddScoped<CreateAddressCommandHandler>();
builder.Services.AddScoped<UpdateAddressCommandHandler>();
builder.Services.AddScoped<PatchAddressCommandHandler>();
builder.Services.AddScoped<DeleteAddressCommandHandler>();
builder.Services.AddScoped<GetUserAddressesQueryHandler>();
builder.Services.AddScoped<GetAddressesQueryHandler>();
builder.Services.AddScoped<GetAddressByIdQueryHandler>();
builder.Services.AddScoped<CreateCurrencyCommandHandler>();
builder.Services.AddScoped<GetCurrenciesQueryHandler>();
builder.Services.AddScoped<GetCurrencyByIdQueryHandler>();
builder.Services.AddScoped<ConvertCurrencyCommandHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Prueba Técnica CLT API v1");
        options.DocumentTitle = "Prueba Técnica CLT API";
        options.DisplayRequestDuration();
        options.EnableTryItOutByDefault();
    });
}

app.MapUserEndpoints();
app.MapAddressEndpoints();
app.MapCurrencyEndpoints();

app.MapFallback(() => Results.Problem(
    statusCode: StatusCodes.Status404NotFound,
    title: "Endpoint no encontrado",
    detail: "La URL solicitada no existe. Verifique la ruta y el metodo HTTP."));

app.Run();

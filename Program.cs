using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PruebaTecnicaClt.Application.Addresses.Commands;
using PruebaTecnicaClt.Application.Addresses.Queries;
using PruebaTecnicaClt.Application.Conversion;
using PruebaTecnicaClt.Application.Currencies.Commands;
using PruebaTecnicaClt.Application.Currencies.Queries;
using PruebaTecnicaClt.Application.Users.Commands;
using PruebaTecnicaClt.Application.Users.Queries;
using PruebaTecnicaClt.Domain.Entities;
using PruebaTecnicaClt.Endpoints;
using PruebaTecnicaClt.Infrastructure.Persistence;
using PruebaTecnicaClt.Middleware;

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
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["ApiKey"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "X-API-KEY",
            In = ParameterLocation.Header,
            Description = "API key required by every business endpoint."
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
builder.Services.AddScoped<DeleteUserCommandHandler>();
builder.Services.AddScoped<GetUsersQueryHandler>();
builder.Services.AddScoped<GetUserByIdQueryHandler>();
builder.Services.AddScoped<CreateAddressCommandHandler>();
builder.Services.AddScoped<UpdateAddressCommandHandler>();
builder.Services.AddScoped<DeleteAddressCommandHandler>();
builder.Services.AddScoped<GetUserAddressesQueryHandler>();
builder.Services.AddScoped<CreateCurrencyCommandHandler>();
builder.Services.AddScoped<GetCurrenciesQueryHandler>();
builder.Services.AddScoped<ConvertCurrencyCommandHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "Prueba Tecnica CLT API v1"));
}

app.MapUserEndpoints();
app.MapAddressEndpoints();
app.MapCurrencyEndpoints();

app.Run();

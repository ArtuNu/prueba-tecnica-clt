using System.Security.Cryptography;
using System.Text;

namespace PruebaTecnicaClt.Middleware;

public sealed class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private const string HeaderName = "X-API-KEY";
    private const string ConfigurationKey = "Security:ApiKey";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/openapi"))
        {
            await next(context);
            return;
        }

        var configuredApiKey = configuration[ConfigurationKey];
        var hasApiKey = context.Request.Headers.TryGetValue(HeaderName, out var providedApiKey);

        if (string.IsNullOrWhiteSpace(configuredApiKey) ||
            !hasApiKey ||
            !KeysMatch(providedApiKey.ToString(), configuredApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                error = $"A valid {HeaderName} header is required."
            });
            return;
        }

        await next(context);
    }

    private static bool KeysMatch(string providedApiKey, string configuredApiKey)
    {
        var providedBytes = Encoding.UTF8.GetBytes(providedApiKey);
        var configuredBytes = Encoding.UTF8.GetBytes(configuredApiKey);

        return CryptographicOperations.FixedTimeEquals(providedBytes, configuredBytes);
    }
}

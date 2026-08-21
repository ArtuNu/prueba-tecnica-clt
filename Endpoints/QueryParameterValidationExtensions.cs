namespace PruebaTecnicaClt.Endpoints;

public static class QueryParameterValidationExtensions
{
    public static RouteHandlerBuilder ValidateQueryParameters(
        this RouteHandlerBuilder builder,
        params string[] allowedParameters)
    {
        return builder
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .AddEndpointFilter(new QueryParameterValidationFilter(allowedParameters));
    }

    private sealed class QueryParameterValidationFilter(string[] allowedParameters) : IEndpointFilter
    {
        private readonly HashSet<string> _allowedParameters =
            new(allowedParameters, StringComparer.OrdinalIgnoreCase);

        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var invalidParameters = context.HttpContext.Request.Query.Keys
                .Where(parameter => !_allowedParameters.Contains(parameter))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (invalidParameters.Length == 0)
            {
                return await next(context);
            }

            var allowedMessage = _allowedParameters.Count == 0
                ? "Este endpoint no admite parámetros de consulta."
                : $"Parámetros permitidos: {string.Join(", ", _allowedParameters.Order())}.";

            var errors = invalidParameters.ToDictionary(
                parameter => parameter,
                parameter => new[]
                {
                    $"El parámetro de consulta '{parameter}' no es válido. {allowedMessage}"
                },
                StringComparer.OrdinalIgnoreCase);

            return Results.ValidationProblem(errors);
        }
    }
}

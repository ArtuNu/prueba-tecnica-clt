using FluentValidation.Results;

namespace PruebaTecnicaClt.Endpoints;

internal static class ValidationExtensions
{
    public static Dictionary<string, string[]> ToErrorDictionary(this ValidationResult result) =>
        result.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).Distinct().ToArray());
}

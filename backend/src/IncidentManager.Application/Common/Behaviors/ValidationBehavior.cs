using FluentValidation;
using MediatR;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior: ejecuta FluentValidation antes de cada Handler.
/// Si hay errores de validación, el Handler nunca se invoca.
/// Devuelve Result.Failure en lugar de lanzar excepción cuando es posible.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any())
            return await next();

        var failures = validators
            .Select(v => v.Validate(new ValidationContext<TRequest>(request)))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var errors = string.Join("; ", failures.Select(f => f.ErrorMessage));

        // Si el tipo de respuesta es Result<T>, devuelve Failure sin excepciones
        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];
            var method    = typeof(Result<>).MakeGenericType(valueType).GetMethod(nameof(Result<object>.Failure))!;
            return (TResponse)method.Invoke(null, [errors])!;
        }

        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(errors);

        throw new ValidationException(failures);
    }
}

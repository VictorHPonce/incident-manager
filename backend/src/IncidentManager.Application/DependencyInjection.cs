using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using IncidentManager.Application.Common.Behaviors;

namespace IncidentManager.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra MediatR handlers, FluentValidation validators y el pipeline de validación.
    /// Llamar desde Program.cs: builder.Services.AddApplication()
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Log temporal — ver qué handlers se registran
        var handlers = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        foreach (var h in handlers)
            Console.WriteLine($"Handler registrado: {h.FullName}");

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

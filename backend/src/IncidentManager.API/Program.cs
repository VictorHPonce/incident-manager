using IncidentManager.API.Endpoints.Auth;
using IncidentManager.API.Endpoints.Incidents;
using IncidentManager.API.Endpoints.Teams;
using IncidentManager.API.Endpoints.Webhooks;
using IncidentManager.API.Middleware;
using IncidentManager.Infrastructure.Persistence;
using IncidentManager.Infrastructure.Extensions;
using Prometheus;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.EntityFrameworkCore;
using IncidentManager.Application;
using IncidentManager.Domain.Auth.Ports;


Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

    // ── Capas ─────────────────────────────────────────────────────────────
    // AddInfrastructure registra internamente AddAuthentication(JwtBearer) 
    // y AddAuthorization() con la configuración completa.
    // No duplicar aquí — el vacío sobreescribiría la configuración JWT.
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── Infraestructura HTTP ───────────────────────────────────────────────
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddOpenApi();

    builder.Services.Configure<RouteOptions>(opts =>
    {
        opts.AppendTrailingSlash = false;
        opts.LowercaseUrls = true;
    });

    builder.Services.ConfigureHttpJsonOptions(opts =>
    {
        opts.SerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DefaultPolicy", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:4200",
                    "https://devexup.net"
                  )
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials() // Necesario si usas Cookies para el Refresh Token
                  .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
    });

    var app = builder.Build();

    app.UseCors("DefaultPolicy");

    // ── Pipeline de middleware — el orden importa ──────────────────────────
    // 1. Captura excepciones (debe ser el primero)
    app.UseExceptionHandler();

    // 2. Logging de requests
    app.UseSerilogRequestLogging();

    // 3. OpenAPI solo en desarrollo
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    // 4. Auth — UseAuthentication ANTES de UseAuthorization, siempre
    //    UseAuthentication: lee el token JWT del header y construye el ClaimsPrincipal
    //    UseAuthorization:  evalúa si el ClaimsPrincipal tiene permiso para el endpoint
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseHttpMetrics();

    // ── Endpoints ──────────────────────────────────────────────────────────
    app.MapHealthChecks("/health");
    app.MapAuthEndpoints();
    app.MapTeamEndpoints();
    app.MapIncidentEndpoints();
    app.MapWebhookEndpoints();
    app.MapMetrics("/metrics");

    Log.Information("Incident Manager arrancando en {Environment}", app.Environment.EnvironmentName);
    // ── Auto-Migración y Configuración Inicial ────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var dbContext = services.GetRequiredService<IncidentManagerDbContext>();

        try
        {
            // Política de reintentos simple para esperar a la DB si está arrancando
            logger.LogInformation("Verificando conexión y aplicando migrations...");

            await dbContext.Database.MigrateAsync();

            logger.LogInformation("Base de datos actualizada y lista.");

            // OPCIONAL: Aquí podrías llamar a un método que ejecute los scripts 
            // si prefieres no meterlos en migraciones de EF.
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error fatal: No se pudo preparar la base de datos.");
            // En producción, es mejor que la app falle si la DB no está lista
            throw;
        }
    }
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "La aplicación falló al arrancar.");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
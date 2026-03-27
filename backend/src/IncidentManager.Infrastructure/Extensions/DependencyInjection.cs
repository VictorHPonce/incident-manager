using IncidentManager.Domain.Auth.Ports;
using IncidentManager.Domain.Incidents.Ports;
using IncidentManager.Domain.Teams.Ports;
using IncidentManager.Infrastructure.Auth;
using IncidentManager.Infrastructure.Auth.Jwt;
using IncidentManager.Infrastructure.Metrics;
using IncidentManager.Infrastructure.Persistence;
using IncidentManager.Infrastructure.Persistence.Repositories.Auth;
using IncidentManager.Infrastructure.Persistence.Repositories.Incidents;
using IncidentManager.Infrastructure.Persistence.Repositories.Teams;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text;

namespace IncidentManager.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // ── PostgreSQL ────────────────────────────────────────────────────────
        services.AddDbContext<IncidentManagerDbContext>(opts =>
    opts.UseNpgsql(
        configuration.GetConnectionString("Postgres"),
        npgsql => npgsql.MigrationsHistoryTable("__ef_migrations", "public"))
    .ConfigureWarnings(w =>
        w.Ignore(RelationalEventId.PendingModelChangesWarning)));

        // ── Redis — Singleton: una conexión compartida en toda la app ─────────
        // StackExchange.Redis gestiona el pool internamente.
        // Crear uno por request (Scoped/Transient) es un error grave de rendimiento.
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("Redis")
                ?? "localhost:6380"));

        // ── JWT Bearer — valida el token en cada request automáticamente ──────
        var secretKey = configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey no configurado");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    // Las 4 validaciones obligatorias en producción
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),

                    // Zero: sin este, tokens expirados se aceptan hasta 5min después
                    ClockSkew = TimeSpan.Zero,

                    // Mapeo de claims estándar a ClaimTypes para que funcione [Authorize(Roles=...)]
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };
            });

        services.AddAuthorization();

        // ── Repositorios ──────────────────────────────────────────────────────
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IIncidentMetrics, PrometheusIncidentMetrics>();

        // ── Servicios de auth ─────────────────────────────────────────────────
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();

        // ── Health checks ─────────────────────────────────────────────────────
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Postgres")!);

        return services;
    }
}
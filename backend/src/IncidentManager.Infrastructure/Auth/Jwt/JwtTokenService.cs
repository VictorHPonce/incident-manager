using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IncidentManager.Domain.Auth.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace IncidentManager.Infrastructure.Auth.Jwt;

internal sealed class JwtTokenService(
    IConnectionMultiplexer redis,
    IConfiguration config)
    : ITokenService
{
    // Lee config una vez y la cachea — evita string lookups en cada llamada
    private string SecretKey => config["Jwt:SecretKey"]!;
    private string Issuer => config["Jwt:Issuer"]!;
    private string Audience => config["Jwt:Audience"]!;
    private int AccessMins => int.Parse(config["Jwt:AccessTokenMins"] ?? "15");
    private int RefreshDays => int.Parse(config["Jwt:RefreshTokenDays"] ?? "7");

    public TokenPair Generate(Guid userId, string email, string role, Guid teamId)
    {
        // Claims que viajan dentro del JWT — el Handler los lee sin ir a la BD
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role,               role),
            new Claim("team_id",                    teamId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(AccessMins),
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        // Refresh token: GUID opaco — no contiene datos, solo es una llave para Redis
        var refreshToken = Guid.NewGuid().ToString("N");

        return new TokenPair(accessToken, refreshToken);
    }

    public Guid? ValidateRefreshToken(string refreshToken)
    {
        // Solo valida sintaxis aquí — la existencia en Redis se comprueba en el Handler
        return Guid.TryParse(refreshToken.Length == 32
            ? refreshToken.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-")
            : refreshToken, out _) ? (Guid?)Guid.Empty : null;
    }

    public async Task StoreRefreshTokenAsync(
        string refreshToken, Guid userId, CancellationToken ct)
    {
        var db = redis.GetDatabase();
        var key = $"refresh:{refreshToken}";
        // TTL nativo de Redis — expira solo, sin cron jobs ni limpieza manual
        await db.StringSetAsync(key, userId.ToString(),
            TimeSpan.FromDays(RefreshDays));
    }

    public async Task RevokeRefreshTokenAsync(
        string refreshToken, CancellationToken ct)
    {
        var db = redis.GetDatabase();
        // DEL atómico — logout inmediato aunque el access token siga vivo hasta expirar
        await db.KeyDeleteAsync($"refresh:{refreshToken}");
    }

    // Usado en RefreshCommandHandler para obtener el userId del token
    public async Task<Guid?> GetUserIdFromRefreshTokenAsync(
    string refreshToken, CancellationToken ct)
    {
        var db = redis.GetDatabase();
        var value = await db.StringGetAsync($"refresh:{refreshToken}");

        // Cast explícito a string — evita la ambigüedad entre las sobrecargas de Guid.TryParse
        var valueStr = (string?)value;
        return valueStr is not null && Guid.TryParse(valueStr, out var id) ? id : null;
    }
}
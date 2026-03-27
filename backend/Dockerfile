# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivos de proyecto usando la ruta relativa desde /backend
COPY ["src/IncidentManager.Domain/IncidentManager.Domain.csproj", "src/IncidentManager.Domain/"]
COPY ["src/IncidentManager.Application/IncidentManager.Application.csproj", "src/IncidentManager.Application/"]
COPY ["src/IncidentManager.Infrastructure/IncidentManager.Infrastructure.csproj", "src/IncidentManager.Infrastructure/"]
COPY ["src/IncidentManager.API/IncidentManager.API.csproj", "src/IncidentManager.API/"]

# Restaurar dependencias
RUN dotnet restore "src/IncidentManager.API/IncidentManager.API.csproj"

# Copiar todo el contenido de /backend y publicar
COPY . .
RUN dotnet publish "src/IncidentManager.API/IncidentManager.API.csproj" \
    -c Release -o /app/publish --no-restore

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Puerto por defecto para modo no-root en .NET 10
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

# Copiar scripts SQL para que la app los use si es necesario
COPY --from=build /src/scripts/db ./scripts/db

# Usuario 'app' de bajas prestaciones por seguridad (nativo en .NET)
USER app

ENTRYPOINT ["dotnet", "IncidentManager.API.dll"]
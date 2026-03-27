# 🌐 Incident Manager API

[![.NET 10](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue.svg)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Plataforma de backend robusta para la gestión de incidentes técnicos, diseñada para integrarse nativamente con stacks de observabilidad (**LGP Stack**) y flujos de trabajo **GitOps**.

---

## 🏗️ Arquitectura de Software
El proyecto sigue los principios de **Clean Architecture**, asegurando un desacoplamiento total entre la lógica de negocio y los detalles de infraestructura.



- **Core/Domain:** Entidades, excepciones de dominio y puertos (interfaces).
- **Core/Application:** Casos de uso, comandos (CQRS) y validaciones con FluentValidation.
- **Infrastructure:** Implementación de persistencia (EF Core), mensajería y servicios externos (Redis, Prometheus).
- **API:** Endpoints minimalistas, middleware de excepciones global y documentación OpenAPI/Scalar.

---

## 🛠️ Stack Tecnológico
- **Lenguaje:** .NET 10 (C#)
- **Base de Datos:** PostgreSQL 17 con Particionamiento y RLS.
- **Caché/Sesiones:** Redis (gestión de Refresh Tokens).
- **Observabilidad:** Prometheus Metrics & Serilog.
- **Seguridad:** JWT Authentication & Row Level Security.

---

## 🚀 Guía de Inicio Rápido

### Requisitos previos
- Docker & Docker Compose
- .NET 10 SDK (para desarrollo local)

### Levantar entorno local
```bash
# 1. Clonar y configurar variables
git clone <url-repo>
cp .env.example .env

# 2. Iniciar servicios (DB, Redis, API)
docker compose up -d

# 3. Comprobar salud del sistema
curl http://localhost:8080/health
```

# Documentación de la API
Una vez iniciada la aplicación en modo Development, la documentación interactiva está disponible en:

Scalar: http://localhost:8080/scalar/v1

OpenAPI JSON: http://localhost:8080/openapi/v1.json

## 🔄 Flujo CI/CD (GitOps)
Este proyecto utiliza Gitea Actions para un despliegue automatizado y seguro:

Build: Compilación y empaquetado en imagen Docker optimizada (distroless/non-root).

Security: Escaneo de dependencias y secretos.

Deploy: Actualización automática en VPS mediante Docker Compose y Traefik.

Mirror: Sincronización automática de la rama main a GitHub para visibilidad pública.

Notify: Notificaciones de estado de despliegue vía Telegram Bot.

## 📈 Roadmap de Desarrollo
[x] Fase 1: Arquitectura base, esquema de base de datos avanzado y Pipeline CI/CD.

[ ] Fase 2: Implementación de Auth (Identity + JWT + Redis).

[ ] Fase 3: Lógica de negocio (Teams & Incidents CRUD).

[ ] Fase 4: Integración de Webhooks para Prometheus Alertmanager.

[ ] Fase 5: Dashboard de Grafana y reportes automáticos.

# Desarrollado con ❤️ por Victor Ponce
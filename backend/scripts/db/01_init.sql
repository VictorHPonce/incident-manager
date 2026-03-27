-- =============================================================================
-- 01_init.sql — Esquemas, tablas y particionado semestral
-- Se ejecuta automáticamente al crear el contenedor PostgreSQL
-- =============================================================================

CREATE EXTENSION IF NOT EXISTS "pgcrypto";  -- gen_random_uuid()
CREATE EXTENSION IF NOT EXISTS "pg_trgm";   -- índices trigramas para búsqueda

CREATE SCHEMA IF NOT EXISTS incidents;
CREATE SCHEMA IF NOT EXISTS teams;
CREATE SCHEMA IF NOT EXISTS auth;

-- Equipos
CREATE TABLE teams.teams (
    id         UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    name       VARCHAR(100) NOT NULL,
    slug       VARCHAR(100) NOT NULL,
    is_active  BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_teams_slug UNIQUE (slug)
);

-- Usuarios
CREATE TABLE auth.users (
    id            UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    email         VARCHAR(255) NOT NULL,
    display_name  VARCHAR(100) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role          VARCHAR(20)  NOT NULL DEFAULT 'Technician'
                               CHECK (role IN ('Technician','TeamLead','Admin')),
    team_id       UUID         NOT NULL REFERENCES teams.teams(id),
    is_active     BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_users_email UNIQUE (email)
);

-- Incidencias — particionada por fecha (semestralmente)
CREATE TABLE incidents.incidents (
    id              UUID         NOT NULL DEFAULT gen_random_uuid(),
    title           VARCHAR(255) NOT NULL,
    description     TEXT,
    severity        VARCHAR(20)  NOT NULL CHECK (severity IN ('Low','Medium','High','Critical')),
    status          VARCHAR(20)  NOT NULL DEFAULT 'Open'
                                 CHECK (status IN ('Open','Acknowledged','InProgress','Resolved','Closed')),
    source_type     VARCHAR(20)  NOT NULL DEFAULT 'Manual'
                                 CHECK (source_type IN ('Manual','Prometheus','Gitea','External')),
    source_ref      VARCHAR(500),
    team_id         UUID         NOT NULL,
    created_by      UUID         NOT NULL,
    assigned_to     UUID,
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    sla_deadline    TIMESTAMPTZ  NOT NULL,
    tags            TEXT[]       NOT NULL DEFAULT '{}',
    source_metadata JSONB        NOT NULL DEFAULT '{}',
    -- Columna generada: full-text search en español sin índice adicional en la query
    search_vector   TSVECTOR     GENERATED ALWAYS AS (
        to_tsvector('spanish', coalesce(title,'') || ' ' || coalesce(description,''))
    ) STORED,
    PRIMARY KEY (id, created_at)  -- created_at en PK requerido por particionado
) PARTITION BY RANGE (created_at);

-- Particiones semestrales — añadir nuevas al inicio de cada semestre
CREATE TABLE incidents.incidents_2025_h1 PARTITION OF incidents.incidents FOR VALUES FROM ('2025-01-01') TO ('2025-07-01');
CREATE TABLE incidents.incidents_2025_h2 PARTITION OF incidents.incidents FOR VALUES FROM ('2025-07-01') TO ('2026-01-01');
CREATE TABLE incidents.incidents_2026_h1 PARTITION OF incidents.incidents FOR VALUES FROM ('2026-01-01') TO ('2026-07-01');
CREATE TABLE incidents.incidents_2026_h2 PARTITION OF incidents.incidents FOR VALUES FROM ('2026-07-01') TO ('2027-01-01');
CREATE TABLE incidents.incidents_future  PARTITION OF incidents.incidents DEFAULT;

-- Historial inmutable de cambios de estado
CREATE TABLE incidents.incident_status_history (
    id          UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    incident_id UUID        NOT NULL,
    from_status VARCHAR(20),
    to_status   VARCHAR(20) NOT NULL,
    changed_by  UUID        NOT NULL,
    changed_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    note        TEXT
);

-- Refresh tokens (respaldo/audit — TTL gestionado en Redis)
CREATE TABLE auth.refresh_tokens (
    id         UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id    UUID         NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    token_hash VARCHAR(255) NOT NULL,
    expires_at TIMESTAMPTZ  NOT NULL,
    revoked_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_refresh_token UNIQUE (token_hash)
);


-- Al final de 01_init.sql — dar ownership a im_user
GRANT ALL ON SCHEMA incidents TO im_user;
GRANT ALL ON SCHEMA teams TO im_user;
GRANT ALL ON SCHEMA auth TO im_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA incidents GRANT ALL ON TABLES TO im_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA teams GRANT ALL ON TABLES TO im_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA auth GRANT ALL ON TABLES TO im_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA incidents GRANT ALL ON SEQUENCES TO im_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA teams GRANT ALL ON SEQUENCES TO im_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA auth GRANT ALL ON SEQUENCES TO im_user;
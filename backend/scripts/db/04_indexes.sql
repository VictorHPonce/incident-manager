-- =============================================================================
-- 04_indexes.sql — Índices optimizados para los patrones de acceso
-- =============================================================================

-- Full-text search en español (sobre la columna generada search_vector)
CREATE INDEX idx_incidents_search ON incidents.incidents USING GIN (search_vector);

-- Tags array: operadores @>, &&
CREATE INDEX idx_incidents_tags ON incidents.incidents USING GIN (tags);

-- JSONB source_metadata: ->, ->>, @>
CREATE INDEX idx_incidents_jsonb ON incidents.incidents USING GIN (source_metadata);

-- Dashboard principal: equipo + estado + fecha (el más frecuente)
CREATE INDEX idx_incidents_team_status ON incidents.incidents (team_id, status, created_at DESC);

-- SLA monitor: solo incidencias abiertas cerca del límite
CREATE INDEX idx_incidents_sla_open ON incidents.incidents (sla_deadline)
    WHERE status NOT IN ('Resolved','Closed');

-- Deduplicación de alertas Prometheus/Gitea
CREATE INDEX idx_incidents_source_ref ON incidents.incidents (source_ref)
    WHERE source_ref IS NOT NULL;

-- Grafana time series
CREATE INDEX idx_incidents_time ON incidents.incidents (created_at DESC);

-- RLS checks por equipo
CREATE INDEX idx_users_team ON auth.users (team_id) WHERE is_active = TRUE;

-- Historial de estados por incidencia
CREATE INDEX idx_status_history ON incidents.incident_status_history (incident_id, changed_at DESC);

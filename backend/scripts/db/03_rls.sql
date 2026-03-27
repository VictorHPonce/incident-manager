-- =============================================================================
-- 03_rls.sql — Row Level Security: aislamiento por equipo
-- La aplicación setea estas variables al inicio de cada request autenticado:
--   SET LOCAL app.current_team_id = '<uuid>';
--   SET LOCAL app.current_role    = 'Technician';
-- Con EF Core: interceptor que llama ExecuteSqlRaw antes de cada operación.
-- =============================================================================

ALTER TABLE incidents.incidents ENABLE ROW LEVEL SECURITY;

-- Admins ven todo
CREATE POLICY policy_admin ON incidents.incidents
    FOR ALL TO PUBLIC
    USING (current_setting('app.current_role', TRUE) = 'Admin');

-- Resto solo ve incidencias de su equipo
CREATE POLICY policy_team_isolation ON incidents.incidents
    FOR ALL TO PUBLIC
    USING (team_id = current_setting('app.current_team_id', TRUE)::UUID);

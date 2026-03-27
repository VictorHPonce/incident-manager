-- =============================================================================
-- 02_triggers.sql — Auditoría de estados y pg_notify para WebSockets (MVP-4)
-- =============================================================================

-- Registra en incident_status_history cada cambio de estado
CREATE OR REPLACE FUNCTION incidents.fn_audit_status_change()
RETURNS TRIGGER AS $$
BEGIN
    IF OLD.status IS DISTINCT FROM NEW.status THEN
        INSERT INTO incidents.incident_status_history
            (incident_id, from_status, to_status, changed_by, changed_at)
        VALUES (NEW.id, OLD.status, NEW.status, NEW.created_by, NOW());
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_audit_incident_status
    AFTER UPDATE ON incidents.incidents
    FOR EACH ROW EXECUTE FUNCTION incidents.fn_audit_status_change();

-- Emite pg_notify en cada cambio — MVP-4: Npgsql LISTEN + SignalR
CREATE OR REPLACE FUNCTION incidents.fn_notify_incident_change()
RETURNS TRIGGER AS $$
BEGIN
    PERFORM pg_notify('incident_updates', json_build_object(
        'incident_id', NEW.id,
        'team_id',     NEW.team_id,
        'status',      NEW.status,
        'severity',    NEW.severity,
        'updated_at',  NEW.updated_at
    )::TEXT);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_notify_incident_change
    AFTER INSERT OR UPDATE ON incidents.incidents
    FOR EACH ROW EXECUTE FUNCTION incidents.fn_notify_incident_change();

-- Actualiza updated_at automáticamente en cada UPDATE
CREATE OR REPLACE FUNCTION public.fn_set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_incidents_updated_at
    BEFORE UPDATE ON incidents.incidents
    FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();

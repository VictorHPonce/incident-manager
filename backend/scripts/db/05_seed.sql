-- =============================================================================
-- 05_seed.sql — Datos de desarrollo. NO ejecutar en producción.
-- Password de todos los usuarios: Dev1234!!
-- Hash generado con BCrypt cost=12
-- =============================================================================

INSERT INTO teams.teams (id, name, slug) VALUES
    ('11111111-0000-0000-0000-000000000001', 'SRE',     'sre'),
    ('11111111-0000-0000-0000-000000000002', 'Backend', 'backend')
ON CONFLICT DO NOTHING;

INSERT INTO auth.users (id, email, display_name, password_hash, role, team_id) VALUES
    ('22222222-0000-0000-0000-000000000001', 'admin@im.local', 'Admin',    '$2a$11$JKYUiaG2ZGkdrydse2YXpuI2BcXPylmU6lOUR0I5sHbiXFsnCnBhK', 'Admin',      '11111111-0000-0000-0000-000000000001'),
    ('22222222-0000-0000-0000-000000000002', 'lead@im.local',  'TeamLead', '$2a$11$JKYUiaG2ZGkdrydse2YXpuI2BcXPylmU6lOUR0I5sHbiXFsnCnBhK', 'TeamLead',   '11111111-0000-0000-0000-000000000001'),
    ('22222222-0000-0000-0000-000000000003', 'tech@im.local',  'Técnico',  '$2a$11$JKYUiaG2ZGkdrydse2YXpuI2BcXPylmU6lOUR0I5sHbiXFsnCnBhK', 'Technician', '11111111-0000-0000-0000-000000000001')
ON CONFLICT DO NOTHING;

INSERT INTO incidents.incidents
    (title, description, severity, status, source_type, team_id, created_by, sla_deadline, tags, source_metadata)
VALUES
    ('CPU alta en prod-api-01', 'CPU al 95% durante 10 min', 'High', 'Open', 'Prometheus',
     '11111111-0000-0000-0000-000000000001', '22222222-0000-0000-0000-000000000001',
     NOW() + INTERVAL '4 hours', ARRAY['prometheus','cpu','prod'],
     '{"alertname":"HighCPU","instance":"prod-api-01","value":"95.2"}'::jsonb),

    ('Pipeline backend fallida', 'Step build falla en rama main', 'Medium', 'Acknowledged', 'Gitea',
     '11111111-0000-0000-0000-000000000002', '22222222-0000-0000-0000-000000000001',
     NOW() + INTERVAL '24 hours', ARRAY['gitea','ci','build'],
     '{"repo":"backend","branch":"main","run_id":"42"}'::jsonb),

    ('Disco casi lleno en DB', 'Uso al 88%', 'Critical', 'InProgress', 'Manual',
     '11111111-0000-0000-0000-000000000001', '22222222-0000-0000-0000-000000000002',
     NOW() + INTERVAL '1 hour', ARRAY['disco','db','prod'],
     '{}'::jsonb)
ON CONFLICT DO NOTHING;

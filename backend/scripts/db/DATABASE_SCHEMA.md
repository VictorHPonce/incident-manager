# 🗄️ Database Architecture: Incident Manager

Este documento detalla la lógica de persistencia avanzada implementada en PostgreSQL 17 para el motor de Incident Manager.

## 📂 Estructura de Objetos SQL

### 01_init.sql: Esquemas y Particionado
- **Segmentación:** Uso de esquemas lógicos (`auth`, `teams`, `incidents`) para separar responsabilidades.
- **Escalabilidad:** Tabla de incidencias particionada por rangos de fecha (`PARTITION BY RANGE`). Esto permite mantener el rendimiento de las consultas a medida que el histórico crece, facilitando el borrado de datos antiguos (retención).

### 02_triggers.sql: Automatización y Reactividad
- **Auditoría:** Trigger `trg_audit_incident_status` que alimenta automáticamente la tabla de histórico ante cualquier cambio de estado.
- **Real-time:** Función `fn_notify_incident_change` que utiliza `pg_notify`. Esto permite que servicios externos o capas de SignalR escuchen cambios en la DB sin hacer polling.

### 03_rls.sql: Seguridad a nivel de Fila (Row Level Security)
- **Aislamiento Multi-tenant:** Implementación de políticas de seguridad que restringen el acceso a los datos basándose en el `current_setting` del equipo y rol del usuario.
- **Capa de Defensa:** La seguridad no depende solo del código C#, sino que el motor de base de datos garantiza que un equipo nunca vea incidentes de otro.

### 04_indexes.sql: Optimización de Búsqueda
- **Full-Text Search:** Índice GIN sobre `search_vector` para búsquedas lingüísticas en español.
- **JSONB:** Indexación de metadatos externos para consultas rápidas sobre payloads de Prometheus y Gitea.

### 05_seed.sql (Desarrollo)
- Datos de prueba iniciales. **Nota:** Las contraseñas en este archivo usan el hash BCrypt para `Dev1234!!` y solo deben ser utilizadas en entornos controlados de staging/dev.
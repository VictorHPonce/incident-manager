using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            var basePath = AppContext.BaseDirectory;

            mb.Sql(File.ReadAllText(Path.Combine(basePath, "scripts/db/01_init.sql")));
            mb.Sql(File.ReadAllText(Path.Combine(basePath, "scripts/db/02_triggers.sql")));
            mb.Sql(File.ReadAllText(Path.Combine(basePath, "scripts/db/03_rls.sql")));
            mb.Sql(File.ReadAllText(Path.Combine(basePath, "scripts/db/04_indexes.sql")));
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DROP TABLE IF EXISTS auth.refresh_tokens;");
            mb.Sql("DROP TABLE IF EXISTS incidents.incident_status_history;");
            mb.Sql("DROP TABLE IF EXISTS incidents.incidents;");
            mb.Sql("DROP TABLE IF EXISTS auth.users;");
            mb.Sql("DROP TABLE IF EXISTS teams.teams;");
            mb.Sql("DROP SCHEMA IF EXISTS incidents;");
            mb.Sql("DROP SCHEMA IF EXISTS auth;");
            mb.Sql("DROP SCHEMA IF EXISTS teams;");
        }
    }
}

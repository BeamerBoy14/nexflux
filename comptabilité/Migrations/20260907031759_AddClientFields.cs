using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comptabilité.Migrations
{
    /// <inheritdoc />
    public partial class AddClientFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent : certaines bases (dev local) ont déjà ces colonnes via add_client_fields.sql.
            migrationBuilder.Sql("IF COL_LENGTH(N'Clients', N'DateNaissance') IS NULL ALTER TABLE [Clients] ADD [DateNaissance] datetime2 NULL;");
            migrationBuilder.Sql("IF COL_LENGTH(N'Clients', N'IdTel') IS NULL ALTER TABLE [Clients] ADD [IdTel] nvarchar(max) NULL;");
            migrationBuilder.Sql("IF COL_LENGTH(N'Clients', N'NumeroIdd') IS NULL ALTER TABLE [Clients] ADD [NumeroIdd] nvarchar(max) NULL;");
            migrationBuilder.Sql("IF COL_LENGTH(N'Clients', N'NumeroPermis') IS NULL ALTER TABLE [Clients] ADD [NumeroPermis] nvarchar(max) NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateNaissance",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IdTel",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "NumeroIdd",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "NumeroPermis",
                table: "Clients");
        }
    }
}

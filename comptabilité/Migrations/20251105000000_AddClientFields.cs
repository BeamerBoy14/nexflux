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
            migrationBuilder.AddColumn<string>(
                name: "NumeroIdd",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroPermis",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateNaissance",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdTel",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroIdd",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "NumeroPermis",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DateNaissance",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IdTel",
                table: "Clients");
        }
    }
}


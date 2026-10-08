using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comptabilité.Migrations
{
    /// <inheritdoc />
    public partial class AddVoitureAnneeKilometrage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Annee",
                table: "Voitures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Kilometrage",
                table: "Voitures",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Annee",
                table: "Voitures");

            migrationBuilder.DropColumn(
                name: "Kilometrage",
                table: "Voitures");
        }
    }
}

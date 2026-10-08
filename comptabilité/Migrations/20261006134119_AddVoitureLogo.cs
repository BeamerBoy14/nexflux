using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comptabilité.Migrations
{
    /// <inheritdoc />
    public partial class AddVoitureLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LogoUpdatedAt",
                table: "Voitures",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VoitureLogos",
                columns: table => new
                {
                    VoitureId = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoitureLogos", x => x.VoitureId);
                    table.ForeignKey(
                        name: "FK_VoitureLogos_Voitures_VoitureId",
                        column: x => x.VoitureId,
                        principalTable: "Voitures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoitureLogos");

            migrationBuilder.DropColumn(
                name: "LogoUpdatedAt",
                table: "Voitures");
        }
    }
}

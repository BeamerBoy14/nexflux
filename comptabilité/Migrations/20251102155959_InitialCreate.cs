using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comptabilité.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodePostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Ville = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Factures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroFacture = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateFacture = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEcheance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MontantHT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TauxTVA = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MontantTVA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontantTTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Factures_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LignesFacture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantite = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontantHT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TauxTVA = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MontantTTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FactureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LignesFacture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LignesFacture_Factures_FactureId",
                        column: x => x.FactureId,
                        principalTable: "Factures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Email",
                table: "Clients",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Factures_ClientId",
                table: "Factures",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Factures_NumeroFacture",
                table: "Factures",
                column: "NumeroFacture",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LignesFacture_FactureId",
                table: "LignesFacture",
                column: "FactureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LignesFacture");

            migrationBuilder.DropTable(
                name: "Factures");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}

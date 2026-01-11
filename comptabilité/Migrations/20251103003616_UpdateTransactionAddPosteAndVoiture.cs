using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comptabilité.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionAddPosteAndVoiture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Créer la table Postes
            migrationBuilder.CreateTable(
                name: "Postes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PeutAvoirVoiture = table.Column<bool>(type: "bit", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Postes_Nom",
                table: "Postes",
                column: "Nom",
                unique: true);

            // Créer la table Voitures
            migrationBuilder.CreateTable(
                name: "Voitures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Immatriculation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voitures", x => x.Id);
                });

            // Supprimer la colonne Categorie de Transactions
            migrationBuilder.DropColumn(
                name: "Categorie",
                table: "Transactions");

            // Ajouter les colonnes PosteId et VoitureId à Transactions
            migrationBuilder.AddColumn<int>(
                name: "PosteId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoitureId",
                table: "Transactions",
                type: "int",
                nullable: true);

            // Créer les index et foreign keys
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PosteId",
                table: "Transactions",
                column: "PosteId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_VoitureId",
                table: "Transactions",
                column: "VoitureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Postes_PosteId",
                table: "Transactions",
                column: "PosteId",
                principalTable: "Postes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Voitures_VoitureId",
                table: "Transactions",
                column: "VoitureId",
                principalTable: "Voitures",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Supprimer les foreign keys
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Voitures_VoitureId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Postes_PosteId",
                table: "Transactions");

            // Supprimer les index
            migrationBuilder.DropIndex(
                name: "IX_Transactions_VoitureId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PosteId",
                table: "Transactions");

            // Supprimer les colonnes ajoutées
            migrationBuilder.DropColumn(
                name: "VoitureId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PosteId",
                table: "Transactions");

            // Recréer la colonne Categorie
            migrationBuilder.AddColumn<string>(
                name: "Categorie",
                table: "Transactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // Supprimer les tables
            migrationBuilder.DropTable(
                name: "Voitures");

            migrationBuilder.DropTable(
                name: "Postes");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comptabilité.Migrations
{
    /// <inheritdoc />
    public partial class AddUtilisateurAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Clients_ClientId",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "Voitures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "Postes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "Factures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomUtilisateur = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MotDePasseHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NomComplet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DerniereConnexion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Voitures_UtilisateurId",
                table: "Voitures",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UtilisateurId",
                table: "Transactions",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UtilisateurId",
                table: "Reservations",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Postes_UtilisateurId",
                table: "Postes",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Factures_UtilisateurId",
                table: "Factures",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UtilisateurId",
                table: "Clients",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_NomUtilisateur",
                table: "Utilisateurs",
                column: "NomUtilisateur",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Utilisateurs_UtilisateurId",
                table: "Clients",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Factures_Utilisateurs_UtilisateurId",
                table: "Factures",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Postes_Utilisateurs_UtilisateurId",
                table: "Postes",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Clients_ClientId",
                table: "Reservations",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Utilisateurs_UtilisateurId",
                table: "Reservations",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Utilisateurs_UtilisateurId",
                table: "Transactions",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Voitures_Utilisateurs_UtilisateurId",
                table: "Voitures",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Utilisateurs_UtilisateurId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Factures_Utilisateurs_UtilisateurId",
                table: "Factures");

            migrationBuilder.DropForeignKey(
                name: "FK_Postes_Utilisateurs_UtilisateurId",
                table: "Postes");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Clients_ClientId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Utilisateurs_UtilisateurId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Utilisateurs_UtilisateurId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Voitures_Utilisateurs_UtilisateurId",
                table: "Voitures");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropIndex(
                name: "IX_Voitures_UtilisateurId",
                table: "Voitures");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UtilisateurId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_UtilisateurId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Postes_UtilisateurId",
                table: "Postes");

            migrationBuilder.DropIndex(
                name: "IX_Factures_UtilisateurId",
                table: "Factures");

            migrationBuilder.DropIndex(
                name: "IX_Clients_UtilisateurId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Voitures");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Postes");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Factures");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Clients");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Clients_ClientId",
                table: "Reservations",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ekklesia.Api.Migrations
{
    /// <inheritdoc />
    public partial class CleanMySQLSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Churches_Users_CreatedBy",
                table: "Churches");

            migrationBuilder.DropForeignKey(
                name: "FK_Churches_Users_UpdatedBy",
                table: "Churches");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Churches_CreatedBy",
                table: "Churches");

            migrationBuilder.DropIndex(
                name: "IX_Churches_UpdatedBy",
                table: "Churches");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Zo7z2H1YgW5FqC5qJ7Q9qu1f3sJ8xK4nL9qW7rE8pV2mN5tA6bC3.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$i7Sat9Rk.4hKyNZaWHUnHeAm9TbJaA38aP33AFYN/C8ExEHixewwi");

            migrationBuilder.CreateIndex(
                name: "IX_Churches_CreatedBy",
                table: "Churches",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Churches_UpdatedBy",
                table: "Churches",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Churches_Users_CreatedBy",
                table: "Churches",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Churches_Users_UpdatedBy",
                table: "Churches",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

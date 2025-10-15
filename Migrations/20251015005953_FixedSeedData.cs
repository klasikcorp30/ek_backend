using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ekklesia.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$i7Sat9Rk.4hKyNZaWHUnHeAm9TbJaA38aP33AFYN/C8ExEHixewwi", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 15, 0, 58, 47, 389, DateTimeKind.Utc).AddTicks(8110), "$2a$11$f3zMKLFSrONNu3jDh4qydeIuIQubhgtQhZcbhoZzrMramSQ3D41yG", new DateTime(2025, 10, 15, 0, 58, 47, 389, DateTimeKind.Utc).AddTicks(8300) });
        }
    }
}

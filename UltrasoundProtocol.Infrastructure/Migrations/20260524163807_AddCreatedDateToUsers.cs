using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UltrasoundProtocol.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedDateToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Users"
                SET "CreatedDate" = "CreatedAt"
                WHERE "CreatedDate" IS NULL
                """);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Users");
        }
    }
}

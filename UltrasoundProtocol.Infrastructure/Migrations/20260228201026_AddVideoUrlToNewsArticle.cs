using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UltrasoundProtocol.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoUrlToNewsArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "NewsArticles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "NewsArticles");
        }
    }
}

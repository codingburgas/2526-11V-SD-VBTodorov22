using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieSeriesCatalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieCoverImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImagePath",
                table: "Movies",
                type: "TEXT",
                maxLength: 260,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImagePath",
                table: "Movies");
        }
    }
}

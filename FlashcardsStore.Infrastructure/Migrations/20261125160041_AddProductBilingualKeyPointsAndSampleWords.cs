using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductBilingualKeyPointsAndSampleWords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackKeyPointsEn",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SampleWords",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackKeyPointsEn",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SampleWords",
                table: "Product");
        }
    }
}

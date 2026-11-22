using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioUrl",
                table: "Product",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsVerifiedBuyer = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StoreNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WhatsAppNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StoreAddressAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    StoreAddressEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    VodafoneCashNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InstaPayAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InstaPayAccountName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PaymentInstructionsAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingFeeCairoGiza = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingFeeDeltaCanal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingFeeUpperEgypt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FreeShippingThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstimatedDeliveryTimeAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AnnouncementBarEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AnnouncementTextAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AnnouncementTextEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AnnouncementLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "StoreSettings");

            migrationBuilder.DropColumn(
                name: "AudioUrl",
                table: "Product");
        }
    }
}

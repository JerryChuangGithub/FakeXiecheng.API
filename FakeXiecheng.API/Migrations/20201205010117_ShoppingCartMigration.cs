using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeXiecheng.API.Migrations
{
    public partial class ShoppingCartMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineItemss",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TouristRouteId = table.Column<Guid>(nullable: false),
                    ShoppingCartId = table.Column<Guid>(nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    DiscountPrice = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItemss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineItemss_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LineItemss_TouristRoutes_TouristRouteId",
                        column: x => x.TouristRouteId,
                        principalTable: "TouristRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13C30197-CDF5-4541-BBFE-3D29BFAD1DDF",
                column: "ConcurrencyStamp",
                value: "b60314c3-672f-4df2-a6da-24112b3b3724");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "DE8AE532-9BAA-49E0-A41C-585812AD3A5B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "99030660-0eaa-40f3-bf1a-0d79f60af113", "AQAAAAEAACcQAAAAELSGH2sqzxp9Dt4I4fOseWHTjW2a5UgvE2P/oh+ibwqPOWiTywQFOpbryu/uBZpNpQ==", "bf516e7a-0051-491c-a567-89080851c7b6" });

            migrationBuilder.CreateIndex(
                name: "IX_LineItemss_ShoppingCartId",
                table: "LineItemss",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItemss_TouristRouteId",
                table: "LineItemss",
                column: "TouristRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_UserId",
                table: "ShoppingCarts",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineItemss");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13C30197-CDF5-4541-BBFE-3D29BFAD1DDF",
                column: "ConcurrencyStamp",
                value: "ca873957-4c6a-4e01-9bf5-eec311fb51fb");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "DE8AE532-9BAA-49E0-A41C-585812AD3A5B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3c826781-0c87-4466-ad03-e6e6c19160fa", "AQAAAAEAACcQAAAAEE4sHV7etr4XHYZ2TNSiSxfHZqmUxUHaVpKh2d50jlMhKJO7i3SoOZK98Kv4nOj9aQ==", "4ee728e3-5acb-4bab-8144-3582b2e4bb21" });
        }
    }
}

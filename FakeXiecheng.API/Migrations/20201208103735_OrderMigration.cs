using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeXiecheng.API.Migrations
{
    public partial class OrderMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "LineItemss",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    CreateDateUTC = table.Column<DateTime>(nullable: false),
                    TransactionMetadata = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13C30197-CDF5-4541-BBFE-3D29BFAD1DDF",
                column: "ConcurrencyStamp",
                value: "a46495c6-1d74-4b29-8f82-988e0900f46c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "DE8AE532-9BAA-49E0-A41C-585812AD3A5B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b67ae3fc-2346-4659-a02d-d0349aea812d", "AQAAAAEAACcQAAAAEIjA+44Im+fWE6BsGliQnt95JL6BQPmLn5w/eDXItaRO4MQVU9zaxjAnU/CTd6r2og==", "48b16044-fcbf-4992-be2c-6848b8716ea9" });

            migrationBuilder.CreateIndex(
                name: "IX_LineItemss_OrderId",
                table: "LineItemss",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItemss_Orders_OrderId",
                table: "LineItemss",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItemss_Orders_OrderId",
                table: "LineItemss");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_LineItemss_OrderId",
                table: "LineItemss");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "LineItemss");

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
        }
    }
}

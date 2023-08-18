using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hubtel.eCommerce.Cart.Infrastructure.Data.Migrations
{
    public partial class Migiration_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ActiveAt",
                table: "User",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "User",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserSession",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(nullable: false),
                    AccessTokenHash = table.Column<string>(nullable: true),
                    AccessTokenExpiresAt = table.Column<DateTimeOffset>(nullable: false),
                    RefreshTokenHash = table.Column<string>(nullable: true),
                    RefreshTokenExpiresAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSession_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UserId",
                table: "UserSession",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSession");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ActiveAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "User");
        }
    }
}

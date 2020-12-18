using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeWeather.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "thID");

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    snID = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ROM = table.Column<string>(nullable: false),
                    CreateAt = table.Column<DateTime>(nullable: true, defaultValueSql: "getdate()"),
                    EditAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.snID);
                });

            migrationBuilder.CreateTable(
                name: "TempHistory",
                columns: table => new
                {
                    thID = table.Column<int>(nullable: false, defaultValueSql: "NEXT VALUE FOR thID"),
                    snID = table.Column<long>(nullable: false),
                    Temperature = table.Column<float>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempHistory", x => x.thID);
                    table.ForeignKey(
                        name: "FK_TempHistory_Sensors_snID",
                        column: x => x.snID,
                        principalTable: "Sensors",
                        principalColumn: "snID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TempHistory_snID",
                table: "TempHistory",
                column: "snID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempHistory");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropSequence(
                name: "thID");
        }
    }
}

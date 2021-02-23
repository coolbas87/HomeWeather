using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeWeather.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "snID");

            migrationBuilder.CreateSequence(
                name: "thID");

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    snID = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR snID"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ROM = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    EditAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.snID);
                });

            migrationBuilder.CreateTable(
                name: "TempHistory",
                columns: table => new
                {
                    thID = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR thID"),
                    snID = table.Column<long>(type: "bigint", nullable: false),
                    Temperature = table.Column<float>(type: "real", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempHistory", x => x.thID);
                    table.ForeignKey(
                        name: "FK_TempHistory_Sensors_snID",
                        column: x => x.snID,
                        principalTable: "Sensors",
                        principalColumn: "snID",
                        onDelete: ReferentialAction.Restrict);
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
                name: "snID");

            migrationBuilder.DropSequence(
                name: "thID");
        }
    }
}

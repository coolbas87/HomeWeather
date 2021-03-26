using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeWeather.Data.Migrations
{
    public partial class TelegramBotUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "tbtuID");

            migrationBuilder.CreateTable(
                name: "TelegramBotTrustedUsers",
                columns: table => new
                {
                    tbtuID = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR tbtuID"),
                    userID = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramBotTrustedUsers", x => x.tbtuID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelegramBotTrustedUsers_userID",
                table: "TelegramBotTrustedUsers",
                column: "userID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramBotTrustedUsers");

            migrationBuilder.DropSequence(
                name: "tbtuID");
        }
    }
}

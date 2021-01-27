using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeWeather.Migrations
{
    public partial class SnIDGen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "snID");

            migrationBuilder.AlterColumn<long>(
                name: "snID",
                table: "Sensors",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR snID",
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "snID");

            migrationBuilder.AlterColumn<long>(
                name: "snID",
                table: "Sensors",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldDefaultValueSql: "NEXT VALUE FOR snID");
        }
    }
}

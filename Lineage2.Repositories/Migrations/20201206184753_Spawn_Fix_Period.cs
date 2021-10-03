using Microsoft.EntityFrameworkCore.Migrations;

namespace Lineage2.Database.Migrations
{
    public partial class Spawn_Fix_Period : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PerdiodOfDay",
                table: "Spawns",
                newName: "PeriodOfDay");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PeriodOfDay",
                table: "Spawns",
                newName: "PerdiodOfDay");
        }
    }
}

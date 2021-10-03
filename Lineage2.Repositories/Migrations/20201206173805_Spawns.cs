using Microsoft.EntityFrameworkCore.Migrations;

namespace Lineage2.Database.Migrations
{
    public partial class Spawns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spawns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpanwnTemplateId = table.Column<int>(type: "int", nullable: false),
                    LocX = table.Column<int>(type: "int", nullable: false),
                    LocY = table.Column<int>(type: "int", nullable: false),
                    LocZ = table.Column<int>(type: "int", nullable: false),
                    Heading = table.Column<int>(type: "int", nullable: false),
                    RespawnDelay = table.Column<int>(type: "int", nullable: false),
                    RespawnRand = table.Column<int>(type: "int", nullable: false),
                    PerdiodOfDay = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spawns", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spawns");
        }
    }
}

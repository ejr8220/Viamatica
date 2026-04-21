using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Viamatica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "navigationmenu",
                columns: table => new
                {
                    navigationmenuid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    menukey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    route = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    displayorder = table.Column<int>(type: "int", nullable: false),
                    isactive = table.Column<bool>(type: "bit", nullable: false),
                    parentnavigationmenuid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_navigationmenu", x => x.navigationmenuid);
                    table.ForeignKey(
                        name: "FK_navigationmenu_navigationmenu_parentnavigationmenuid",
                        column: x => x.parentnavigationmenuid,
                        principalTable: "navigationmenu",
                        principalColumn: "navigationmenuid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "navigationmenurol",
                columns: table => new
                {
                    navigationmenuid = table.Column<int>(type: "int", nullable: false),
                    rolid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_navigationmenurol", x => new { x.navigationmenuid, x.rolid });
                    table.ForeignKey(
                        name: "FK_navigationmenurol_navigationmenu_navigationmenuid",
                        column: x => x.navigationmenuid,
                        principalTable: "navigationmenu",
                        principalColumn: "navigationmenuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_navigationmenurol_rol_rolid",
                        column: x => x.rolid,
                        principalTable: "rol",
                        principalColumn: "rolid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_navigationmenu_menukey",
                table: "navigationmenu",
                column: "menukey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_navigationmenu_parentnavigationmenuid",
                table: "navigationmenu",
                column: "parentnavigationmenuid");

            migrationBuilder.CreateIndex(
                name: "IX_navigationmenurol_rolid",
                table: "navigationmenurol",
                column: "rolid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "navigationmenurol");

            migrationBuilder.DropTable(
                name: "navigationmenu");
        }
    }
}

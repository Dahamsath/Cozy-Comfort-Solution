using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CozyComfort.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDistributorInventoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DistributorInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BlanketId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributorInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistributorInventories_Blankets_BlanketId",
                        column: x => x.BlanketId,
                        principalTable: "Blankets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistributorInventories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistributorInventories_BlanketId",
                table: "DistributorInventories",
                column: "BlanketId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorInventories_UserId",
                table: "DistributorInventories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistributorInventories");
        }
    }
}

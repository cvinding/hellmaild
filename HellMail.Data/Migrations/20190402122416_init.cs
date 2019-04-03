using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HellMail.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    firstname = table.Column<string>(nullable: true),
                    surname = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Mails",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    subject = table.Column<string>(nullable: true),
                    message = table.Column<string>(nullable: true),
                    UserDestinationId = table.Column<int>(nullable: false),
                    UserSourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mails", x => x.id);
                    table.ForeignKey(
                        name: "FK_Mails_Users_UserDestinationId",
                        column: x => x.UserDestinationId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mails_Users_UserSourceId",
                        column: x => x.UserSourceId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mails_UserDestinationId",
                table: "Mails",
                column: "UserDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Mails_UserSourceId",
                table: "Mails",
                column: "UserSourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mails");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

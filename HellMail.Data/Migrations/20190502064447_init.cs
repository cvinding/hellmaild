using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HellMail.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mails",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    subject = table.Column<string>(nullable: true),
                    message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mails", x => x.id);
                });

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
                name: "Hidden_Mails",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mail_id = table.Column<int>(nullable: true),
                    owner_id = table.Column<int>(nullable: true),
                    hidden = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hidden_Mails", x => x.id);
                    table.ForeignKey(
                        name: "FK_Hidden_Mails_Mails_mail_id",
                        column: x => x.mail_id,
                        principalTable: "Mails",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hidden_Mails_Users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mails_Users",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mail_id = table.Column<int>(nullable: true),
                    from_user_id = table.Column<int>(nullable: true),
                    to_user_id = table.Column<int>(nullable: true),
                    recipient_type = table.Column<int>(nullable: false, defaultValue: 0),
                    hidden = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mails_Users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Mails_Users_Users_from_user_id",
                        column: x => x.from_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mails_Users_Mails_mail_id",
                        column: x => x.mail_id,
                        principalTable: "Mails",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mails_Users_Users_to_user_id",
                        column: x => x.to_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hidden_Mails_mail_id",
                table: "Hidden_Mails",
                column: "mail_id");

            migrationBuilder.CreateIndex(
                name: "IX_Hidden_Mails_owner_id",
                table: "Hidden_Mails",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_Mails_Users_from_user_id",
                table: "Mails_Users",
                column: "from_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Mails_Users_mail_id",
                table: "Mails_Users",
                column: "mail_id");

            migrationBuilder.CreateIndex(
                name: "IX_Mails_Users_to_user_id",
                table: "Mails_Users",
                column: "to_user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hidden_Mails");

            migrationBuilder.DropTable(
                name: "Mails_Users");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Mails");
        }
    }
}

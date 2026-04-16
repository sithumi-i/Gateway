using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gateway.BlindMatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ResearchAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectProposals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Abstract = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    TechnicalStack = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ResearchAreaId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentId = table.Column<string>(type: "TEXT", nullable: false),
                    SupervisorId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProposals_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectProposals_AspNetUsers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectProposals_ResearchAreas_ResearchAreaId",
                        column: x => x.ResearchAreaId,
                        principalTable: "ResearchAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupervisorExpertises",
                columns: table => new
                {
                    SupervisorId = table.Column<string>(type: "TEXT", nullable: false),
                    ResearchAreaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupervisorExpertises", x => new { x.SupervisorId, x.ResearchAreaId });
                    table.ForeignKey(
                        name: "FK_SupervisorExpertises_AspNetUsers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupervisorExpertises_ResearchAreas_ResearchAreaId",
                        column: x => x.ResearchAreaId,
                        principalTable: "ResearchAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposals_ResearchAreaId",
                table: "ProjectProposals",
                column: "ResearchAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposals_StudentId",
                table: "ProjectProposals",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposals_SupervisorId",
                table: "ProjectProposals",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisorExpertises_ResearchAreaId",
                table: "SupervisorExpertises",
                column: "ResearchAreaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectProposals");

            migrationBuilder.DropTable(
                name: "SupervisorExpertises");

            migrationBuilder.DropTable(
                name: "ResearchAreas");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");
        }
    }
}

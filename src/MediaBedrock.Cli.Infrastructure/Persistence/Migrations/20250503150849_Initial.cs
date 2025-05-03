using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaBedrock.Cli.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Author = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Inputs = table.Column<string>(type: "TEXT", nullable: true),
                    Outputs = table.Column<string>(type: "TEXT", nullable: true),
                    Properties = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Inputs = table.Column<string>(type: "TEXT", nullable: true),
                    Outputs = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_JobTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "JobTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobTemplateSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ProcessorName = table.Column<string>(type: "TEXT", nullable: false),
                    Inputs = table.Column<string>(type: "TEXT", nullable: true),
                    Outputs = table.Column<string>(type: "TEXT", nullable: true),
                    Properties = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTemplateSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobTemplateSteps_JobTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "JobTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobsStateMachines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExecutionStatus = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsStateMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobsStateMachines_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessorName = table.Column<string>(type: "TEXT", nullable: false),
                    Inputs = table.Column<string>(type: "TEXT", nullable: true),
                    Outputs = table.Column<string>(type: "TEXT", nullable: true),
                    Properties = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSteps_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobStateMachineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Uri = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    Kind = table.Column<string>(type: "TEXT", nullable: false),
                    MediaInformation_Format = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobAssets_JobsStateMachines_JobStateMachineId",
                        column: x => x.JobStateMachineId,
                        principalTable: "JobsStateMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobStepStateMachines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobStateMachineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StepName = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessorName = table.Column<string>(type: "TEXT", nullable: false),
                    ExecutionStatus = table.Column<string>(type: "TEXT", nullable: false),
                    ExecutionError_Kind = table.Column<string>(type: "TEXT", nullable: true),
                    ExecutionError_Message = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    StepInputs = table.Column<string>(type: "TEXT", nullable: true),
                    StepOutputs = table.Column<string>(type: "TEXT", nullable: true),
                    StepProperties = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStepStateMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobStepStateMachines_JobsStateMachines_JobStateMachineId",
                        column: x => x.JobStateMachineId,
                        principalTable: "JobsStateMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobAssets_JobStateMachineId",
                table: "JobAssets",
                column: "JobStateMachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_TemplateId",
                table: "Jobs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_JobsStateMachines_JobId",
                table: "JobsStateMachines",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSteps_JobId",
                table: "JobSteps",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobStepStateMachines_JobStateMachineId",
                table: "JobStepStateMachines",
                column: "JobStateMachineId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTemplateSteps_TemplateId",
                table: "JobTemplateSteps",
                column: "TemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobAssets");

            migrationBuilder.DropTable(
                name: "JobSteps");

            migrationBuilder.DropTable(
                name: "JobStepStateMachines");

            migrationBuilder.DropTable(
                name: "JobTemplateSteps");

            migrationBuilder.DropTable(
                name: "JobsStateMachines");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "JobTemplates");
        }
    }
}

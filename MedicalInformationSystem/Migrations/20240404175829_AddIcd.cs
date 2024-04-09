using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInformationSystem.Migrations
{
    public partial class AddIcd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IcdDiagnosisNewId",
                table: "Diagnosis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "IcdRoots",
                columns: table => new
                {
                    NewId = table.Column<Guid>(type: "uuid", nullable: false),
                    ID = table.Column<int>(type: "integer", nullable: false),
                    MKB_CODE = table.Column<string>(type: "text", nullable: false),
                    MKB_NAME = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcdRoots", x => x.NewId);
                });

            migrationBuilder.CreateTable(
                name: "Icd",
                columns: table => new
                {
                    NewId = table.Column<Guid>(type: "uuid", nullable: false),
                    ID = table.Column<int>(type: "integer", nullable: false),
                    MKB_CODE = table.Column<string>(type: "text", nullable: false),
                    MKB_NAME = table.Column<string>(type: "text", nullable: false),
                    ID_PARENT = table.Column<int>(type: "integer", nullable: false),
                    IcdRootNewId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icd", x => x.NewId);
                    table.ForeignKey(
                        name: "FK_Icd_IcdRoots_IcdRootNewId",
                        column: x => x.IcdRootNewId,
                        principalTable: "IcdRoots",
                        principalColumn: "NewId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnosis_IcdDiagnosisNewId",
                table: "Diagnosis",
                column: "IcdDiagnosisNewId");

            migrationBuilder.CreateIndex(
                name: "IX_Icd_IcdRootNewId",
                table: "Icd",
                column: "IcdRootNewId");

            migrationBuilder.CreateIndex(
                name: "IX_Icd_MKB_CODE",
                table: "Icd",
                column: "MKB_CODE");

            migrationBuilder.CreateIndex(
                name: "IX_Icd_MKB_NAME",
                table: "Icd",
                column: "MKB_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_Icd_NewId",
                table: "Icd",
                column: "NewId");

            migrationBuilder.CreateIndex(
                name: "IX_IcdRoots_MKB_CODE",
                table: "IcdRoots",
                column: "MKB_CODE");

            migrationBuilder.CreateIndex(
                name: "IX_IcdRoots_MKB_NAME",
                table: "IcdRoots",
                column: "MKB_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_IcdRoots_NewId",
                table: "IcdRoots",
                column: "NewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnosis_Icd_IcdDiagnosisNewId",
                table: "Diagnosis",
                column: "IcdDiagnosisNewId",
                principalTable: "Icd",
                principalColumn: "NewId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnosis_Icd_IcdDiagnosisNewId",
                table: "Diagnosis");

            migrationBuilder.DropTable(
                name: "Icd");

            migrationBuilder.DropTable(
                name: "IcdRoots");

            migrationBuilder.DropIndex(
                name: "IX_Diagnosis_IcdDiagnosisNewId",
                table: "Diagnosis");

            migrationBuilder.DropColumn(
                name: "IcdDiagnosisNewId",
                table: "Diagnosis");
        }
    }
}

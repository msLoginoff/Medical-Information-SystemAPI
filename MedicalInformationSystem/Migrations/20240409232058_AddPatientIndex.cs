using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInformationSystem.Migrations
{
    public partial class AddPatientIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Patient_Name",
                table: "Patient",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patient_Name",
                table: "Patient");
        }
    }
}

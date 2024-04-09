using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInformationSystem.Migrations
{
    public partial class addIndexesForDoctorAndSpeciality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Speciality_Name",
                table: "Speciality",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_Email",
                table: "Doctor",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Speciality_Name",
                table: "Speciality");

            migrationBuilder.DropIndex(
                name: "IX_Doctor_Email",
                table: "Doctor");
        }
    }
}

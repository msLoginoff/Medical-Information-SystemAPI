using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInformationSystem.Migrations
{
    public partial class addCorrectForeignKeyForPasswordEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Password",
                table: "Password");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Password",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Password",
                table: "Password",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Password_DoctorId",
                table: "Password",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Password_Doctor_DoctorId",
                table: "Password",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Password_Doctor_DoctorId",
                table: "Password");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Password",
                table: "Password");

            migrationBuilder.DropIndex(
                name: "IX_Password_DoctorId",
                table: "Password");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Password");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Password",
                table: "Password",
                column: "DoctorId");
        }
    }
}

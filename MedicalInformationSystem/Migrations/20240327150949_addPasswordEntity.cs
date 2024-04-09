using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInformationSystem.Migrations
{
    public partial class addPasswordEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Doctor");

            migrationBuilder.CreateTable(
                name: "Password",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    HashedPassword = table.Column<string>(type: "text", nullable: false),
                    Salt = table.Column<string>(type: "text", nullable: false),
                    TokenSeries = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Password", x => x.DoctorId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Password");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Doctor",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

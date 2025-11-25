using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMvc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "phonenumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phonenumber",
                table: "Students");
        }
    }
}

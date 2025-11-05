using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWD_SoftwareArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxCapacityToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxCapacity",
                table: "Courses",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "Courses");
        }
    }
}

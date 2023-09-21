using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsumerManager.Migrations
{
    /// <inheritdoc />
    public partial class RenamingSurname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Surename",
                table: "Customers",
                newName: "Surname");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Customers",
                newName: "Surename");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace icpc_modle.Migrations
{
    /// <inheritdoc />
    public partial class addex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Percentages",
                table: "ExamResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Percentages",
                table: "ExamResults");
        }
    }
}

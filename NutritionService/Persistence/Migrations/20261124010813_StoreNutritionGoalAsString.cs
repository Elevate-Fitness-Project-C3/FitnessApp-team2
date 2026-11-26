using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutritionService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StoreNutritionGoalAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NutritionGoal",
                table: "NutritionPreferences",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NutritionGoal",
                table: "NutritionPreferences",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingService.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutTitleUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Workouts_Title",
                table: "Workouts",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workouts_Title",
                table: "Workouts");
        }
    }
}

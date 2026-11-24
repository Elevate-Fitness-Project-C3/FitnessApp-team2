using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessProfileService.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnOnboardingStatusToFitnessProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OnboardingStatus",
                table: "FitnessProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnboardingStatus",
                table: "FitnessProfiles");
        }
    }
}

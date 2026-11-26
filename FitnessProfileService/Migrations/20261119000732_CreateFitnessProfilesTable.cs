using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessProfileService.Migrations
{
    /// <inheritdoc />
    public partial class CreateFitnessProfilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FitnessProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    PrimaryFitnessGoal = table.Column<int>(type: "int", nullable: false),
                    ActivityLevel = table.Column<int>(type: "int", nullable: false),
                    TrainingExperienceLevel = table.Column<int>(type: "int", nullable: true),
                    AvailableDaysPerWeek = table.Column<int>(type: "int", maxLength: 7, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FitnessProfiles", x => x.UserId);
                    table.CheckConstraint("CK_FitnessProfiles_DateOfBirth", "[DateOfBirth] > '1900-01-01' AND [DateOfBirth] < CAST(GETDATE() AS DATE)");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FitnessProfiles");
        }
    }
}

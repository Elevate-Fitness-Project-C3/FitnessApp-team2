using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutritionService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNutritionPreferenceIdToSnowflake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NutritionPreferences",
                table: "NutritionPreferences");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "NutritionPreferences");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "NutritionPreferences",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NutritionPreferences",
                table: "NutritionPreferences",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NutritionPreferences",
                table: "NutritionPreferences");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "NutritionPreferences");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "NutritionPreferences",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_NutritionPreferences",
                table: "NutritionPreferences",
                column: "Id");
        }
    }
}
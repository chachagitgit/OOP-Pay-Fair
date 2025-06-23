using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OOP_Fair_Fare.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedRouteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AppliedDiscount",
                table: "SavedRoutes",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "SavedRoutes",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RegularFare",
                table: "SavedRoutes",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Vehicle",
                table: "SavedRoutes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliedDiscount",
                table: "SavedRoutes");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "SavedRoutes");

            migrationBuilder.DropColumn(
                name: "RegularFare",
                table: "SavedRoutes");

            migrationBuilder.DropColumn(
                name: "Vehicle",
                table: "SavedRoutes");
        }
    }
}

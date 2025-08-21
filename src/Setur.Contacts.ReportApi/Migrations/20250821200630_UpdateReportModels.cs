using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setur.Contacts.ReportApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Parameters",
                table: "Reports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Reports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmailCount",
                table: "ReportDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parameters",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "EmailCount",
                table: "ReportDetails");
        }
    }
}

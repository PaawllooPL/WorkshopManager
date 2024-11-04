using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshopManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class OwnerCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Client_FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Client_LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Client_FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Client_LastName",
                table: "AspNetUsers");
        }
    }
}

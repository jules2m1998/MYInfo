using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYInfo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_meta_data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone_numbers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    social_links = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    last_modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    last_modified_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_activated = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_meta_data", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_meta_data");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class Discord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "discord_id",
                table: "player",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "discord_verification_code",
                table: "player",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "discord_id",
                table: "player");

            migrationBuilder.DropColumn(
                name: "discord_verification_code",
                table: "player");
        }
    }
}

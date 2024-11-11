using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diary.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryptionKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "EncryptedKey",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedKey",
                table: "AspNetUsers");
        }
    }
}

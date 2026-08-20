using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prueba_tecnica_clt.Migrations
{
    /// <inheritdoc />
    public partial class CaseInsensitiveUniqueFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                maxLength: 254,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 254);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Currencies",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                maxLength: 254,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 254,
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Currencies",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 3,
                oldCollation: "NOCASE");
        }
    }
}

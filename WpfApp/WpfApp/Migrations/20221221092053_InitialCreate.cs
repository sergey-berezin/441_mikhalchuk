using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WpfApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Embedding = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bytedatas",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "INTEGER", nullable: false),
                    byteimage = table.Column<byte[]>(name: "byte_image", type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bytedatas", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_Bytedatas_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hashkeys",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "INTEGER", nullable: false),
                    hashkey = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hashkeys", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_Hashkeys_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bytedatas");

            migrationBuilder.DropTable(
                name: "Hashkeys");

            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}

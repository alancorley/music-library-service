using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicLibrary.API.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Genre = table.Column<string>(maxLength: 100, nullable: true),
                    Filename = table.Column<string>(maxLength: 500, nullable: true),
                    ArtistId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Songs_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "FirstName", "LastName" },
                values: new object[,]
                {
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"), "Elvis", "Presley" },
                    { new Guid("da2fd609-d754-4feb-8acd-c4f9ff13ba96"), "Bob", "Dylan" },
                    { new Guid("2902b665-1190-4c70-9915-b9c2d7680450"), "Katy", "Perry" },
                    { new Guid("102b566b-ba1f-404c-b2df-e2cde39ade09"), "Snoop", "Dogg" },
                    { new Guid("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"), "Bruno", "Mars" },
                    { new Guid("2aadd2df-7caf-45ab-9355-7f6332985a87"), "Johnny", "Cash" },
                    { new Guid("2ee49fe3-edf2-4f91-8409-3eb25ce6ca51"), "Jay", "Z" }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "ArtistId", "Filename", "Genre", "Title" },
                values: new object[,]
                {
                    { new Guid("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"), new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"), "", "Rock and Roll", "Hound Dog" },
                    { new Guid("d8663e5e-7494-4f81-8739-6e0de1bea7ee"), new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"), "", "Rock and Roll", "Viva Las Vegas" },
                    { new Guid("d173e20d-159e-4127-9ce9-b0ac2564ad97"), new Guid("da2fd609-d754-4feb-8acd-c4f9ff13ba96"), "", "Rock and Roll", "Knocking on Heaven's Door" },
                    { new Guid("40ff5488-fdab-45b5-bc3a-14302d59869a"), new Guid("2902b665-1190-4c70-9915-b9c2d7680450"), "", "Pop", "Roar" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_ArtistId",
                table: "Songs",
                column: "ArtistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "Artists");
        }
    }
}

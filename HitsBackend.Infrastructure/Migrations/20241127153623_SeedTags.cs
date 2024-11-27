using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name", "CreateTime" },
                values: new object[,]
                {
                    { Guid.NewGuid(), "история", DateTime.UtcNow },
                    { Guid.NewGuid(), "еда", DateTime.UtcNow },
                    { Guid.NewGuid(), "18+", DateTime.UtcNow },
                    { Guid.NewGuid(), "приколы", DateTime.UtcNow },
                    { Guid.NewGuid(), "it", DateTime.UtcNow },
                    { Guid.NewGuid(), "интернет", DateTime.UtcNow },
                    { Guid.NewGuid(), "теория_заговора", DateTime.UtcNow },
                    { Guid.NewGuid(), "соцсети", DateTime.UtcNow },
                    { Guid.NewGuid(), "косплей", DateTime.UtcNow },
                    { Guid.NewGuid(), "преступление", DateTime.UtcNow }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Name",
                keyValues: new object[]
                {
                    "история", "еда", "18+", "приколы", "it",
                    "интернет", "теория_заговора", "соцсети", "косплей", "преступление"
                });
        }
    }
}

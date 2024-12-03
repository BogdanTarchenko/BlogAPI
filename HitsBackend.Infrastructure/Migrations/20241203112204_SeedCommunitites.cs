using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCommunitites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Communities",
                columns: new[] { "Id", "Name", "Description", "IsClosed", "SubscribersCount", "CreateTime" },
                values: new object[,]
                {
                    { Guid.NewGuid(), "Масонская ложа", "Место, помещение, где собираются масоны для проведения своих собраний, чаще называемых работами", true, 0, DateTime.UtcNow },
                    { Guid.NewGuid(), "Следствие вели с Л. Каневским", "Без длинных предисловий: мужчина умер", false, 0, DateTime.UtcNow },
                    { Guid.NewGuid(), "IT <3", "Информационные технологии связаны с изучением методов и средств сбора, обработки и передачи данных с целью получения информации нового качества о состоянии объекта, процесса или явления", false, 0, DateTime.UtcNow }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Communities",
                keyColumn: "Name",
                keyValues: new object[]
                {
                    "Масонская ложа",
                    "Следствие вели с Л. Каневским",
                    "IT <3"
                });
        }
    }
}

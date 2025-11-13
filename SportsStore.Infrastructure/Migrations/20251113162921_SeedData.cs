using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportsStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "Name" },
                values: new object[,]
                {
                    { 1, "Bóng đá" },
                    { 2, "Quần áo" },
                    { 3, "Cầu lông" },
                    { 4, "Giày" },
                    { 5, "Bóng chuyền" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductID", "Category", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Bóng đá", 1, "Bóng đá dùng cho mọi loại sân, độ bền cao.", null, "Bóng đá Adidas", 25.00m },
                    { 2, "Quần áo", 2, "Áo đấu chất liệu thoáng khí, thấm hút mồ hôi.", null, "Áo đấu đội tuyển A", 49.99m },
                    { 3, "Cầu lông", 3, "Vợt nhẹ, cân bằng tốt, phù hợp cho người chơi trung cấp.", null, "Vợt cầu lông Yonex", 120.00m },
                    { 4, "Giày", 4, "Giày êm ái, hỗ trợ tối đa cho các buổi tập dài.", null, "Giày chạy bộ Nike Air", 95.00m },
                    { 5, "Bóng chuyền", 5, "Bóng chuyền tiêu chuẩn thi đấu quốc tế.", null, "Bóng chuyền Mikasa MVA200", 30.00m },
                    { 6, "Bóng đá", 1, "Bóng đá tập luyện chất lượng cao.", null, "Bóng đá Nike Strike", 20.00m },
                    { 7, "Quần áo", 2, "Quần short co giãn, thoải mái khi vận động.", null, "Quần short thể thao", 25.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 5);
        }
    }
}

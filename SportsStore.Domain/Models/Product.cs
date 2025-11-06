using System.ComponentModel.DataAnnotations; // Để dùng [Key]
namespace SportsStore.Domain.Models
{
    // Class Product đại diện cho một sản phẩm trong cửa hàng
    public class Product
    {
        [Key] // Đánh dấu đây là khóa chính
        public int ProductID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}

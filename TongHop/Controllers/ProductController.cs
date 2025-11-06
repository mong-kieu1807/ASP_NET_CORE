// Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Models;
using Microsoft.Extensions.Options; // Cần thiết để sử dụng IOptions
using Microsoft.Extensions.Logging; // Cần thiết để sử dụng ILogger
using TongHop.Configurations; 
using System.Linq;
// Áp dụng một route tiền tố cho toàn bộ Controller nếu muốn dùng Attribute Routing mạnh mẽ
// [Route("san-pham")] // Ví dụ: mọi action sẽ bắt đầu bằng /cua-hang/
public class ProductController : Controller
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductController> _logger;
 private readonly PagingSettings _pagingSettings; // Khai báo thuộc tính để lưu cấu hình phân trang
    public ProductController(IProductRepository repository, ILogger<ProductController> logger, IOptions<PagingSettings> pagingSettings)
    {
        _repository = repository;
        _logger = logger;
        _pagingSettings = pagingSettings.Value;
    }
    // Ví dụ 1: Convention-based Routing (không có [Route] attribute ở đây)
    // Sẽ khớp với /Product/List hoặc /Product (nếu List là action mặc định của ProductController)
    public IActionResult List(string? category = null, int productPage = 1) // Tham số category để lọc, có thể null
    {
        _logger.LogInformation("Yêu cầu danh sách sản phẩm. Danh mục: {Category}, Trang: {Page}", category, productPage);
        int itemsPerPage = _pagingSettings.ItemsPerPage;
        var productsQuery = _repository.Products
            .Where(p => category == null || p.Category == category);
        var products = productsQuery
            .OrderBy(p => p.ProductID) 
            .Skip((productPage - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToList();
        ViewBag.CurrentCategory = category ?? "Tất cả sản phẩm";
        ViewBag.CurrentPage = productPage;
        ViewBag.TotalItems = productsQuery.Count();
        ViewBag.ItemsPerPage = itemsPerPage;
        //_logger.LogInformation("Trả về {ProductCount} sản phẩm cho trang {Page}. Tổng số sản phẩm: {TotalItems}", products.Count, productPage, ViewBag.TotalItems);
        return View(products);
    }
    // Ví dụ 2: Action này sẽ được gọi bởi route "product_by_category" trong Program.cs
    // public IActionResult ListByCategory(string category) { /* Logic tương tự List(category) */ }
    // (Chúng ta có thể gộp logic vào một Action `List` duy nhất như trên)
    // Ví dụ 3: Attribute Routing cho chi tiết sản phẩm
    // Sẽ khớp với /product/chi-tiet/{id}
    // [Route("product/chi-tiet/{id:int}")] // Nếu không có tiền tố
    // Controller Route
    [Route("chi-tiet/{id:int}")] // Nếu có [Route("product")] ở cấp Controller
    public IActionResult Details(int id)
    {
        var product = _repository.Products.FirstOrDefault(p => p.ProductID == id);
        if (product == null)
        {
            _logger.LogWarning("Không tìm thấy sản phẩm với ID:{ ProductID}.", id);
            return NotFound();
        }
        _logger.LogInformation("Hiển thị chi tiết sản phẩm ID:{ProductID}", id);
        return View(product);
    }
    // Tạo một Action để kiểm tra ghi log lỗi
    public IActionResult SimulateError()
    {
        try
        {
            _logger.LogWarning("Mô phỏng lỗi để kiểm tra nhật ký...");
            throw new InvalidOperationException("Đây là lỗi kiểm tra từ SimulateError action!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Đã xảy ra lỗi không mong muốn trong quá trình mô phỏng lỗi!");
        }
        return Content("Kiểm tra đầu ra console/debug của bạn để tìm nhật ký!");

    }
}
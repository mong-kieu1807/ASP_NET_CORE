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
        // Lấy danh sách tất cả categories duy nhất để hiển thị sidebar
        ViewBag.Categories = _repository.Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
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
    // Action Edit (GET): Hiển thị form chỉnh sửa sản phẩm
    public IActionResult Edit(int? productId)
    {
        // Kiểm tra productId có giá trị không
        if (productId == null || productId == 0)
        {
            _logger.LogWarning("ProductId null hoặc 0 khi gọi Edit.");
            TempData["message"] = "ID sản phẩm không hợp lệ.";
            return RedirectToAction("List");
        }

        // Tìm sản phẩm theo ID
        Product? product = _repository.Products
            .FirstOrDefault(p => p.ProductID == productId.Value);
        if (product == null)
        {
            // Nếu không tìm thấy sản phẩm, chuyển hướng về List
            _logger.LogWarning("Không tìm thấy sản phẩm có ID {ProductId} để chỉnh sửa.", productId);
            TempData["message"] = $"Sản phẩm có ID {productId} không tồn tại.";
            return RedirectToAction("List");
        }
        return View(product); // Truyền đối tượng Product vào View
    }
    // Action Edit (POST): Xử lý dữ liệu gửi từ form chỉnh sửa
    [HttpPost]
    public IActionResult Edit(Product product) // Model Binding sẽ tự động điền dữ liệu vào 'product'
    {
        if (ModelState.IsValid) // Kiểm tra xem Model có hợp lệ không
        {
            _repository.SaveProduct(product); // Lưu sản phẩm (sẽ tạo mới nếu ID = 0, cập nhật nếu ID > 0)
            _logger.LogInformation("Product '{Sản phẩmName}'(ID: {ProductId}) đã được lưu thành công.", product.Name, product.ProductID);
            TempData["message"] = $"{product.Name} đã được lưu thành công!"; // Hiển thị thông báo
            return RedirectToAction("List"); // Chuyển hướng về trang danh sách sản phẩm
        }
        else
        {
            // Dữ liệu không hợp lệ, trả về View để hiển thị lỗi
            _logger.LogWarning("Không xác thực được sản phẩm'{ProductName}'(ID: { ProductId}). Lỗi: { Errors}", product.Name ?? "N / A", product.ProductID,
            string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return View(product); // Truyền Model trở lại View để giữ dữ liệu đã nhập
        }
    }
    // Action Create: Hiển thị form tạo sản phẩm mới (tương tự Edit nhưng ID = 0)
    public ViewResult Create()
    {
        return View("Edit", new Product()); // Sử dụng lại View Edit với một Product trống
    }
    // Action Delete: Giả lập xóa sản phẩm (chỉ để hoàn thiện logic cơ bản)
    [HttpPost]
    public IActionResult Delete(int productId)
    {
        Product? productToDelete =
        _repository.Products.FirstOrDefault(p => p.ProductID == productId);
        if (productToDelete != null)
        {
            // FakeProductRepository không có Remove, nên chỉ log
            _logger.LogInformation("Sản phẩm '{ProductName}' (ID: { ProductId}) được đánh dấu để xóa(thực tế không bị xóa trong FakeRepository).", productToDelete.Name, productToDelete.ProductID);
            TempData["message"] = $"{productToDelete.Name} đã được đánh dấu xóa!";
        }
        else
        {
            TempData["message"] = $"Sản phẩm có ID {productId} không tồn tại để xóa.";
        }
        return RedirectToAction("List");
    }
    public IActionResult FilterProducts(ProductFilter filter) // Model Binding cho ProductFilter
    {
        _logger.LogInformation("Lọc sản phẩn theo Category: {Category}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, InStockOnly: {InStock} ",
        filter.Category, filter.MinPrice,
        filter.MaxPrice, filter.InStockOnly);
        // Logic lọc sản phẩm dựa trên filter
        var filteredProducts = _repository.Products;
        if (!string.IsNullOrEmpty(filter.Category))
        {
            filteredProducts = filteredProducts.Where(p => p.Category ==
            filter.Category);
        }
        if (filter.MinPrice.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.Price >=
        filter.MinPrice.Value);
        }
        if (filter.MaxPrice.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.Price <=
            filter.MaxPrice.Value);
        }
        // Nếu InStockOnly = true, thì lọc thêm điều kiện này
        // if (filter.InStockOnly) { filteredProducts =
        // filteredProducts.Where(p => p.IsInStock()); }
        
        // Set ViewBag để tránh lỗi null trong List view
        ViewBag.CurrentCategory = filter.Category ?? "Tất cả sản phẩm";
        ViewBag.CurrentPage = 1;
        ViewBag.TotalItems = filteredProducts.Count();
        ViewBag.ItemsPerPage = _pagingSettings.ItemsPerPage;
        ViewBag.Categories = _repository.Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
        
        return View("List", filteredProducts.ToList()); // Tái sử dụng ViewList
    }
}
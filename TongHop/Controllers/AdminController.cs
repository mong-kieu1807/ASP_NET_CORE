using Microsoft.AspNetCore.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SportsStore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
// Giả sử bạn có IProductRepository được tiêm ở đây

[Authorize(Roles = "Admin")]
public class AdminController : Controller // Hoặc AdminController
{
    private readonly IProductRepository _repository;
    private readonly ILogger<AdminController> _logger;
    private readonly ApplicationDbContext _context;
    public AdminController(IProductRepository repository, ILogger<AdminController> logger, ApplicationDbContext context)
    {
        _repository = repository;
        _logger = logger;
        _context = context;
    }
    public IActionResult List(string? category = null, int productPage = 1)
    {
        var products = _repository.Products.ToList();
        
        // Thiết lập ViewBag cho phân trang
        ViewBag.CurrentCategory = category ?? "Tất cả sản phẩm";
        ViewBag.CurrentPage = productPage;
        ViewBag.TotalItems = products.Count;
        ViewBag.ItemsPerPage = 10; // Hoặc lấy từ PagingSettings
        ViewBag.Categories = _repository.Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
        
        return View("List", products);
    }
    // Action để hiển thị form tạo/chỉnh sửa sản phẩm
    public async Task<IActionResult> Edit(int productId = 0)
    {
        Product? product = productId == 0 ? new Product() :
        await _repository.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        if (product == null && productId != 0)
        {
            _logger.LogWarning("Không tìm thấy sản phẩm có ID {ProductID} để chỉnh sửa.", productId);
            return NotFound();
        }
        
        // Load danh sách categories cho dropdown
        ViewBag.Categories = new SelectList(
            await _context.Categories.ToListAsync(), 
            "CategoryID", 
            "Name", 
            product?.CategoryId
        );
        
        return View(product);
    }
    // Action xử lý POST khi người dùng gửi form
    [HttpPost]
    [ValidateAntiForgeryToken] // Quan trọng cho bảo mật
    public async Task<IActionResult> Edit(Product product)
    {
        // Lấy tên category từ CategoryId TRƯỚC khi validate
        var category = await _context.Categories.FindAsync(product.CategoryId);
        if (category != null)
        {
            product.Category = category.Name;
        }
        
        if (ModelState.IsValid) // Kiểm tra hợp lệ dựa trên Data Annotations
        {
            await _repository.SaveProduct(product);
            _logger.LogInformation("Dữ liệu sản phẩm cho '{ProductName}' hợp lệ. Đã lưu thành công.", product.Name);
            TempData["message"] = $"{product.Name} đã được lưu thành công!";
            return RedirectToAction("List", "Admin");
        }
        else
        {
            // Có lỗi validation, hiển thị lại form với các thông báo lỗi
            _logger.LogWarning("Dữ liệu sản phẩm cho '{ProductName}' không hợp lệ.Lỗi xác thực: { Errors} ", product.Name,
            string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            
            // Load lại categories cho dropdown khi có lỗi
            ViewBag.Categories = new SelectList(
                await _context.Categories.ToListAsync(), 
                "CategoryID", 
                "Name", 
                product.CategoryId
            );
            
            return View(product); // Trả về cùng View với Model có lỗi
        }
    }
    
    // Action Create
    public async Task<IActionResult> Create()
    {
        // Load danh sách categories cho dropdown
        ViewBag.Categories = new SelectList(
            await _context.Categories.ToListAsync(), 
            "CategoryID", 
            "Name"
        );
        
        return View("Edit", new Product());
    }
    
    // Action Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int productId)
    {
        Product? deletedProduct = await _repository.DeleteProduct(productId);
        if (deletedProduct != null)
        {
            _logger.LogInformation("Đã xóa sản phẩm '{ProductName}' (ID: {ProductId}).", deletedProduct.Name, deletedProduct.ProductID);
            TempData["message"] = $"{deletedProduct.Name} đã được xóa thành công!";
        }
        else
        {
            TempData["message"] = $"Không tìm thấy sản phẩm có ID {productId} để xóa.";
        }
        return RedirectToAction("List", "Admin");
    }
}
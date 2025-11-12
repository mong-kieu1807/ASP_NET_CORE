using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TongHop.Models;
using SportsStore.Domain.Abstract;

namespace TongHop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductRepository _repository;
    private readonly PagingSettings _pagingSettings; // Khai báo thuộc tính để lưu cấu hình phân trang

    public HomeController(ILogger<HomeController> logger, IProductRepository repository, IOptions<PagingSettings> pagingSettings)
    {
        _logger = logger;
        _repository = repository;
        _pagingSettings = pagingSettings.Value;
    }

    public IActionResult Index(string? category = null, int productPage = 1)
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
        return View(products);
    }
    public IActionResult AboutUs()
    {
        _logger.LogInformation("Yêu cầu trang Giới thiệu.");
        // Có thể truyền một thông điệp đơn giản
        ViewBag.Message = "Đây là trang giới thiệu về chúng tôi.";
        return View(); // Trả về Views/Home/AboutUs.cshtml (cần tạo)
    }
    public IActionResult GetServerTime()
    {
        _logger.LogInformation("Đã yêu cầu thời gian máy chủ.");
        return Json(new { Time = DateTime.Now.ToString("HH:mm:ss"), Date = DateTime.Now.ToShortDateString() });
    }
    // Ví dụ chuyển hướng
    public IActionResult GoToProductList()
    {
        _logger.LogInformation("Đang chuyển hướng đến danh sách sản phẩm.");
        return RedirectToAction("List", "Product"); // Chuyển hướng đến ProductController.List
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    // Action để giả lập lỗi 500
    public IActionResult SimulateFatalError()
    {
        throw new InvalidOperationException("Đây là một ngoại lệ được cố ý đưa ra để kiểm tra cách xử lý lỗi!!");
    }
}

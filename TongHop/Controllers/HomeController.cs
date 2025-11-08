using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TongHop.Models;

namespace TongHop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Đã yêu cầu trang chủ.");
        return View();
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

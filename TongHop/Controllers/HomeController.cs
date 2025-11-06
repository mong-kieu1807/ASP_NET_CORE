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
        return View();
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

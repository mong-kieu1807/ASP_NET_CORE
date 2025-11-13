using SportsStore.Domain.Abstract;
using TongHop.Concrete;
using TongHop.Configurations;
using Microsoft.EntityFrameworkCore;
using SportsStore.Infrastructure.Data;
using SportsStore.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SportsStoreConnection")));

// Đăng ký PagingSettings để có thể inject IOptions<PagingSettings> vào Controller
builder.Services.Configure<PagingSettings>(builder.Configuration.GetSection("PagingSettings"));
builder.Services.AddScoped<IProductRepository, EFProductRepository>(); // Thay thế bằng EFProductRepository

// Đăng ký IHttpContextAccessor để truy cập HttpContext trong ViewComponent
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Đăng ký Middleware tùy chỉnh đầu tiên trong pipeline
app.UseMiddleware<TongHop.Middleware.RequestLoggerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Hiển thị trang lỗi chi tiết với stack trace
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Trang lỗi tùy chỉnh cho Production
    app.UseHsts(); // HSTS cho Production
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "category_page",
    pattern: "{category}/Page{productPage:int}", // Mẫu URL: {category}(tên danh mục) / Page{productPage} (số trang)
    defaults: new { Controller = "Product", action = "List" } // Controller và Action mặc định
);
app.MapControllerRoute(
 name: "pagination",
 pattern: "Page{productPage:int}", // Mẫu URL: Page{productPage}
 defaults: new { Controller = "Product", action = "List" }
);
app.MapControllerRoute(
 name: "category",
 pattern: "{category}", // Mẫu URL: {category} (tên danh mục)
 defaults: new
 {
     Controller = "Product",
     action = "List",
     productPage
= 1
 } // Mặc định là trang 1
);
app.MapControllerRoute(
 name: "default",
 pattern: "{controller=Product}/{action=List}/{id?}");

app.Run();

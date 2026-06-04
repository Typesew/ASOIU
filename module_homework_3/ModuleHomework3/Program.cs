using Microsoft.EntityFrameworkCore;
using ModuleHomework3.Data;
using ModuleHomework3.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=products.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Category { Name = "Электроника" },
            new Category { Name = "Бытовая техника" },
            new Category { Name = "Одежда" },
            new Category { Name = "Книги" }
        );
        context.SaveChanges();
    }

    if (!context.Products.Any())
    {
        var cats = context.Categories.ToDictionary(c => c.Name, c => c.Id);
        context.Products.AddRange(
            new Product { CategoryId = cats["Электроника"], Name = "Смартфон XYZ", Price = 15000 },
            new Product { CategoryId = cats["Электроника"], Name = "Ноутбук ABC", Price = 45000 },
            new Product { CategoryId = cats["Электроника"], Name = "Наушники QWE", Price = 3000 },
            new Product { CategoryId = cats["Бытовая техника"], Name = "Холодильник RTY", Price = 35000 },
            new Product { CategoryId = cats["Бытовая техника"], Name = "Микроволновка UIO", Price = 8000 },
            new Product { CategoryId = cats["Бытовая техника"], Name = "Пылесос PAS", Price = 12000 },
            new Product { CategoryId = cats["Одежда"], Name = "Куртка зимняя", Price = 5000 },
            new Product { CategoryId = cats["Одежда"], Name = "Джинсы", Price = 2500 },
            new Product { CategoryId = cats["Одежда"], Name = "Футболка", Price = 800 },
            new Product { CategoryId = cats["Книги"], Name = "Война и мир", Price = 1200 },
            new Product { CategoryId = cats["Книги"], Name = "Преступление и наказание", Price = 900 },
            new Product { CategoryId = cats["Книги"], Name = "Мастер и Маргарита", Price = 1100 }
        );
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

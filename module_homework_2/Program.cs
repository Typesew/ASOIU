using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// ══════════════════════════════════════════════════════════
// Точка входа — консольное меню
// ══════════════════════════════════════════════════════════

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string dbPath = "shop.db";
string categoriesCsv = Path.Combine("Data", "categories.csv");
string productsCsv = Path.Combine("Data", "products.csv");

var db = new DatabaseManager(dbPath);
db.InitializeDatabase(categoriesCsv, productsCsv);

Console.WriteLine();

string choice;
do
{
    Console.WriteLine("╔══════════════════════════════════════════╗");
    Console.WriteLine("║         УПРАВЛЕНИЕ ТОВАРАМИ              ║");
    Console.WriteLine("╠══════════════════════════════════════════╣");
    Console.WriteLine("║  1 — Показать все категории             ║");
    Console.WriteLine("║  2 — Показать все товары                 ║");
    Console.WriteLine("║  3 — Добавить товар                      ║");
    Console.WriteLine("║  4 — Редактировать товар                 ║");
    Console.WriteLine("║  5 — Удалить товар                       ║");
    Console.WriteLine("║  6 — Отчёты                              ║");
    Console.WriteLine("║  7 — Фильтр по категории [ГРУППА Г]      ║");
    Console.WriteLine("║  8 — Экспорт в CSV [ГРУППА Б]            ║");
    Console.WriteLine("║  0 — Выход                               ║");
    Console.WriteLine("╚══════════════════════════════════════════╝");
    Console.Write("Ваш выбор: ");
    
    choice = Console.ReadLine()?.Trim() ?? "";
    Console.WriteLine();
    
    switch (choice)
    {
        case "1": ShowCategories(db); break;
        case "2": ShowProducts(db); break;
        case "3": AddProduct(db); break;
        case "4": EditProduct(db); break;
        case "5": DeleteProduct(db); break;
        case "6": ReportsMenu(db); break;
        case "7": FilterByCategory(db); break;
        case "8": ExportCsv(db); break;
        case "0": Console.WriteLine("До свидания!"); break;
        default: Console.WriteLine("Неверный пункт меню."); break;
    }
    Console.WriteLine();
} while (choice != "0");

// ========== Функции пунктов меню ==========

static void ShowCategories(DatabaseManager db)
{
    Console.WriteLine("---- Все категории ----");
    var categories = db.GetAllCategories();
    foreach (var cat in categories)
        Console.WriteLine("  " + cat);
    Console.WriteLine($"Итого: {categories.Count}");
}

static void ShowProducts(DatabaseManager db)
{
    Console.WriteLine("---- Все товары ----");
    var products = db.GetAllProducts();
    foreach (var prod in products)
        Console.WriteLine("  " + prod);
    Console.WriteLine($"Итого: {products.Count}");
}

static void AddProduct(DatabaseManager db)
{
    Console.WriteLine("---- Добавление товара ----");
    
    Console.WriteLine("Доступные категории:");
    var categories = db.GetAllCategories();
    foreach (var cat in categories)
        Console.WriteLine("  " + cat);
    
    Console.Write("ID категории: ");
    if (!int.TryParse(Console.ReadLine(), out int catId))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    
    Console.Write("Название товара: ");
    string name = Console.ReadLine()?.Trim() ?? "";
    if (name.Length == 0)
    {
        Console.WriteLine("Ошибка: название не может быть пустым.");
        return;
    }
    
    Console.Write("Цена (руб.): ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal price))
    {
        Console.WriteLine("Ошибка: введите число.");
        return;
    }
    
    try
    {
        var product = new Product(0, catId, name, price);
        db.AddProduct(product);
        Console.WriteLine("Товар добавлен.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

static void EditProduct(DatabaseManager db)
{
    Console.WriteLine("---- Редактирование товара ----");
    Console.Write("Введите ID товара: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    
    var product = db.GetProductById(id);
    if (product == null)
    {
        Console.WriteLine($"Товар с ID={id} не найден.");
        return;
    }
    
    Console.WriteLine($"Текущие данные: {product}");
    Console.WriteLine("(Нажмите Enter, чтобы оставить значение без изменений)");
    
    Console.Write($"Название [{product.Name}]: ");
    string input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0)
        product.Name = input;
    
    Console.Write($"ID категории [{product.CategoryId}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && int.TryParse(input, out int newCatId))
        product.CategoryId = newCatId;
    
    Console.Write($"Цена [{product.Price}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && decimal.TryParse(input, out decimal newPrice))
    {
        try
        {
            product.Price = newPrice;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return;
        }
    }
    
    db.UpdateProduct(product);
    Console.WriteLine("Данные обновлены.");
}

static void DeleteProduct(DatabaseManager db)
{
    Console.WriteLine("---- Удаление товара ----");
    Console.Write("Введите ID товара: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    
    var product = db.GetProductById(id);
    if (product == null)
    {
        Console.WriteLine($"Товар с ID={id} не найден.");
        return;
    }
    
    Console.Write($"Удалить «{product.Name}»? (да/нет): ");
    string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
    if (confirm == "да")
    {
        db.DeleteProduct(id);
        Console.WriteLine("Товар удалён.");
    }
    else
    {
        Console.WriteLine("Удаление отменено.");
    }
}

// ========== Подменю отчётов ==========

static void ReportsMenu(DatabaseManager db)
{
    string choice;
    do
    {
        Console.WriteLine("--- Отчёты ---");
        Console.WriteLine(" 1 - Товары по категориям");
        Console.WriteLine(" 2 - Количество товаров в категориях");
        Console.WriteLine(" 3 - Средняя цена товаров по категориям");
        Console.WriteLine(" 0 - Назад");
        Console.Write("Ваш выбор: ");
        choice = Console.ReadLine()?.Trim() ?? "";
        
        switch (choice)
        {
            case "1": Report1_ProductsWithCategories(db); break;
            case "2": Report2_CountByCategory(db); break;
            case "3": Report3_AvgPriceByCategory(db); break;
            case "0": break;
            default: Console.WriteLine("Неверный пункт."); break;
        }
        Console.WriteLine();
    } while (choice != "0");
}

// Отчёт 1: Товары с названиями категорий (JOIN)
static void Report1_ProductsWithCategories(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT p.prod_name, c.cat_name, p.price
                 FROM products p
                 JOIN categories c ON p.cat_id = c.cat_id
                 ORDER BY p.prod_name")
        .Title("Товары по категориям")
        .Header("Товар", "Категория", "Цена (руб.)")
        .ColumnWidths(25, 20, 12)
        .Numbered()
        .Footer("Всего товаров")
        .Print();
}

// Отчёт 2: Количество товаров по категориям (GROUP BY + COUNT)
static void Report2_CountByCategory(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT c.cat_name, COUNT(*) AS cnt
                 FROM products p
                 JOIN categories c ON p.cat_id = c.cat_id
                 GROUP BY c.cat_name
                 ORDER BY c.cat_name")
        .Title("Количество товаров по категориям")
        .Header("Категория", "Кол-во")
        .ColumnWidths(25, 10)
        .Print();
}

// Отчёт 3: Средняя цена товаров по категориям (GROUP BY + AVG)
static void Report3_AvgPriceByCategory(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT c.cat_name, ROUND(AVG(p.price), 2) AS avg_price
                 FROM products p
                 JOIN categories c ON p.cat_id = c.cat_id
                 GROUP BY c.cat_name
                 ORDER BY avg_price DESC")
        .Title("Средняя цена товаров по категориям")
        .Header("Категория", "Средняя цена (руб.)")
        .ColumnWidths(25, 20)
        .Print();
}

// [ГРУППА Г] Фильтр по категории
static void FilterByCategory(DatabaseManager db)
{
    Console.WriteLine("---- Фильтр по категории ----");
    Console.WriteLine("Доступные категории:");
    var categories = db.GetAllCategories();
    foreach (var cat in categories)
        Console.WriteLine("  " + cat);
    
    Console.Write("Введите ID категории: ");
    if (!int.TryParse(Console.ReadLine(), out int catId))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    
    var products = db.GetProductsByCategory(catId);
    if (products.Count == 0)
    {
        Console.WriteLine("В этой категории нет товаров.");
        return;
    }
    
    Console.WriteLine($"\nТовары в категории #{catId}:");
    foreach (var prod in products)
        Console.WriteLine("  " + prod);
    Console.WriteLine($"Итого: {products.Count}");
}

// [ГРУППА Б] Экспорт в CSV
static void ExportCsv(DatabaseManager db)
{
    string catPath = Path.Combine("Data", "categories_export.csv");
    string prodPath = Path.Combine("Data", "products_export.csv");
    db.ExportToCsv(catPath, prodPath);
    Console.WriteLine($"Категории экспортированы в: {catPath}");
    Console.WriteLine($"Товары экспортированы в: {prodPath}");
}

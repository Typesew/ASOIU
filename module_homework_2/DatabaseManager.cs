using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Управление базой данных SQLite.
/// Инкапсулирует все операции с БД: создание таблиц,
/// импорт CSV, CRUD-операции, выполнение запросов для отчётов.
/// </summary>
class DatabaseManager
{
    private string _connectionString;
    
    /// <summary>
    /// Конструктор. Принимает путь к файлу БД.
    /// </summary>
    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }
    
    // ========== Инициализация ==========
    
    /// <summary>
    /// Создаёт таблицы (если не существуют) и загружает CSV при первом запуске
    /// </summary>
    public void InitializeDatabase(string categoriesCsvPath, string productsCsvPath)
    {
        CreateTables();
        
        if (GetAllCategories().Count == 0 && File.Exists(categoriesCsvPath))
        {
            ImportCategoriesFromCsv(categoriesCsvPath);
            Console.WriteLine($"[OK] Загружены категории из {categoriesCsvPath}");
        }
        
        if (GetAllProducts().Count == 0 && File.Exists(productsCsvPath))
        {
            ImportProductsFromCsv(productsCsvPath);
            Console.WriteLine($"[OK] Загружены товары из {productsCsvPath}");
        }
    }
    
    /// <summary>Создание таблиц</summary>
    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS categories (
                cat_id INTEGER PRIMARY KEY AUTOINCREMENT,
                cat_name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS products (
                prod_id INTEGER PRIMARY KEY AUTOINCREMENT,
                cat_id INTEGER NOT NULL,
                prod_name TEXT NOT NULL,
                price INTEGER NOT NULL,
                FOREIGN KEY (cat_id) REFERENCES categories(cat_id)
            );";
        cmd.ExecuteNonQuery();
    }
    
    /// <summary>Импорт категорий из CSV</summary>
    private void ImportCategoriesFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 2) continue;
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO categories (cat_id, cat_name) VALUES (@id, @name)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
        }
    }
    
    /// <summary>Импорт товаров из CSV</summary>
    private void ImportProductsFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 4) continue;
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO products (prod_id, cat_id, prod_name, price)
                VALUES (@id, @catId, @name, @price)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@catId", int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@name", parts[2]);
            cmd.Parameters.AddWithValue("@price", int.Parse(parts[3]));
            cmd.ExecuteNonQuery();
        }
    }
    
    // ========== Чтение данных ==========
    
    /// <summary>Получить все категории</summary>
    public List<Category> GetAllCategories()
    {
        var result = new List<Category>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT cat_id, cat_name FROM categories ORDER BY cat_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Category(reader.GetInt32(0), reader.GetString(1)));
        }
        return result;
    }
    
    /// <summary>Получить все товары</summary>
    public List<Product> GetAllProducts()
    {
        var result = new List<Product>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT prod_id, cat_id, prod_name, price FROM products ORDER BY prod_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Product(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3)
            ));
        }
        return result;
    }
    
    /// <summary>Получить товар по Id</summary>
    public Product GetProductById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT prod_id, cat_id, prod_name, price FROM products WHERE prod_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Product(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3)
            );
        }
        return null;
    }
    
    // ========== Изменение данных ==========
    
    /// <summary>Добавить товар (Id генерируется автоматически)</summary>
    public void AddProduct(Product product)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO products (cat_id, prod_name, price)
            VALUES (@catId, @name, @price)";
        cmd.Parameters.AddWithValue("@catId", product.CategoryId);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.ExecuteNonQuery();
    }
    
    /// <summary>Обновить данные товара</summary>
    public void UpdateProduct(Product product)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE products
            SET cat_id = @catId, prod_name = @name, price = @price
            WHERE prod_id = @id";
        cmd.Parameters.AddWithValue("@id", product.Id);
        cmd.Parameters.AddWithValue("@catId", product.CategoryId);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.ExecuteNonQuery();
    }
    
    /// <summary>Удалить товар по Id</summary>
    public void DeleteProduct(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM products WHERE prod_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
    
    // ========== Выполнение произвольного запроса (для отчётов) ==========
    
    /// <summary>
    /// Выполняет SQL-запрос и возвращает имена столбцов и строки результата.
    /// Используется классом ReportBuilder.
    /// </summary>
    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();
        
        string[] columns = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
            columns[i] = reader.GetName(i);
        
        var rows = new List<string[]>();
        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i)?.ToString() ?? "";
            rows.Add(row);
        }
        return (columns, rows);
    }
    
    // ========== [ГРУППА Б] Экспорт в CSV ==========
    
    /// <summary>Экспорт обеих таблиц в CSV-файлы</summary>
    public void ExportToCsv(string categoriesPath, string productsPath)
    {
        var catLines = new List<string> { "cat_id;cat_name" };
        foreach (var cat in GetAllCategories())
            catLines.Add($"{cat.Id};{cat.Name}");
        File.WriteAllLines(categoriesPath, catLines.ToArray());
        
        var prodLines = new List<string> { "prod_id;cat_id;prod_name;price" };
        foreach (var prod in GetAllProducts())
            prodLines.Add($"{prod.Id};{prod.CategoryId};{prod.Name};{prod.Price}");
        File.WriteAllLines(productsPath, prodLines.ToArray());
    }
    
    // ========== [ГРУППА Г] Фильтр по категории ==========
    
    /// <summary>Получить товары конкретной категории</summary>
    public List<Product> GetProductsByCategory(int catId)
    {
        var result = new List<Product>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT prod_id, cat_id, prod_name, price
            FROM products WHERE cat_id = @catId ORDER BY prod_name";
        cmd.Parameters.AddWithValue("@catId", catId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Product(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3)
            ));
        }
        return result;
    }
}

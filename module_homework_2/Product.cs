using System;

/// <summary>
/// Товар (основная таблица, сторона «много»)
/// </summary>
class Product
{
    /// <summary>Идентификатор товара</summary>
    public int Id { get; set; }
    
    /// <summary>Идентификатор категории (внешний ключ)</summary>
    public int CategoryId { get; set; }
    
    /// <summary>Название товара</summary>
    public string Name { get; set; }
    
    private decimal _price;
    
    /// <summary>
    /// Цена товара в рублях (не может быть отрицательной)
    /// </summary>
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentException("Цена не может быть отрицательной");
            _price = value;
        }
    }
    
    /// <summary>Конструктор с параметрами</summary>
    public Product(int id, int categoryId, string name, decimal price)
    {
        Id = id;
        CategoryId = categoryId;
        Name = name;
        Price = price;
    }
    
    /// <summary>Конструктор по умолчанию</summary>
    public Product() : this(0, 0, "", 0) { }
    
    public override string ToString() => $"[{Id}] {Name}, категория #{CategoryId}, цена: {Price} руб.";
}

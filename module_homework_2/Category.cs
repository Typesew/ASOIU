/// <summary>
/// Категория товаров (справочная таблица, сторона «один»)
/// </summary>
class Category
{
    /// <summary>Идентификатор категории</summary>
    public int Id { get; set; }
    
    /// <summary>Название категории</summary>
    public string Name { get; set; }
    
    /// <summary>Конструктор с параметрами</summary>
    public Category(int id, string name)
    {
        Id = id;
        Name = name;
    }
    
    /// <summary>Конструктор по умолчанию</summary>
    public Category() : this(0, "") { }
    
    public override string ToString() => $"[{Id}] {Name}";
}

namespace ElectronicStore.Models;

// Товар (таблица products)
public partial class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;
}

using DomainCore.DomainObjects;

namespace OrderApi.DbContexts.OrderDb.Entities;

public class Product : Entity
{
    public int OrderId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;

    #region Relationships

    public virtual Order Order { get; set; }

    #endregion

    public Product()
    {
    }

    public Product(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}
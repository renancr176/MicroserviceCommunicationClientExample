using DomainCore.DomainObjects;

namespace ProductApi.DbContexts.ProductDb.Entities;

public class Product : Entity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool Active { get; set; }

    public Product()
    {
    }

    public Product(string name, decimal price, bool active)
    {
        Name = name;
        Price = price;
        Active = active;
    }
}
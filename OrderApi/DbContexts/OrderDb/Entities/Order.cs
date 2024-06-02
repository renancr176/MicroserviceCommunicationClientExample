using DomainCore.DomainObjects;

namespace OrderApi.DbContexts.OrderDb.Entities;

public class Order : EntityIntId
{
    public Guid CustomerId { get; set; }
    public decimal Total => Products?.Sum(p => p.Total) ?? 0;

    #region Relationships

    public virtual ICollection<Product> Products { get; set; }

    #endregion

    public Order()
    {
    }

    public Order(Guid customerId, List<Product> products)
    {
        CustomerId = customerId;
        Products = products;
    }
}
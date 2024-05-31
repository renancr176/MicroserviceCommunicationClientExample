using OrderApi.DbContexts.OrderDb.Entities;
using OrderApi.DbContexts.OrderDb.Interfaces.Repositories;

namespace OrderApi.DbContexts.OrderDb.Repositories;

public class OrderRepository : OrderDbRepositoryIntId<Order>, IOrderRepository
{
    public OrderRepository(OrderDbContext context) : base(context)
    {
    }
}
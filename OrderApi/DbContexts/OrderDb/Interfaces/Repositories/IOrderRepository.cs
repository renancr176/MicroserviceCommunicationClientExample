using DomainCore.Data;
using OrderApi.DbContexts.OrderDb.Entities;

namespace OrderApi.DbContexts.OrderDb.Interfaces.Repositories;

public interface IOrderRepository : IRepositoryIntId<Order>
{
}
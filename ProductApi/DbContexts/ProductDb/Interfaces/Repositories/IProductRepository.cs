using DomainCore.Data;
using ProductApi.DbContexts.ProductDb.Entities;

namespace ProductApi.DbContexts.ProductDb.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
}
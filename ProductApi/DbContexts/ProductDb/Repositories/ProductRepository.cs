using ProductApi.DbContexts.ProductDb.Entities;
using ProductApi.DbContexts.ProductDb.Interfaces.Repositories;

namespace ProductApi.DbContexts.ProductDb.Repositories;

public class ProductRepository : ProductDbRepository<Product>, IProductRepository
{
    public ProductRepository(ProductDbContext context) : base(context)
    {
    }
}
using Bogus;
using ProductApi.DbContexts.ProductDb.Entities;
using ProductApi.DbContexts.ProductDb.Interfaces.Repositories;
using ProductApi.DbContexts.ProductDb.Interfaces.Seeders;

namespace ProductApi.DbContexts.ProductDb.Seeders;

public class ProductSeeder : IProductSeeder
{
    private readonly IProductRepository _productRepository;

    public ProductSeeder(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task SeedAsync()
    {
        if (!await _productRepository.AnyAsync(x => true))
        {
            for (int i = 0; i < 100; i++)
            {
                Product product;

                do
                {
                    var faker = new Faker();
                    product = new Product(faker.Commerce.ProductName(), faker.Random.Decimal(0.25M, 99.99M), true);
                } while (await _productRepository.AnyAsync(x => x.Name.ToLower() == product.Name.ToLower()));

                await _productRepository.InsertAsync(product);
                await _productRepository.SaveChangesAsync();
            }
        }
    }
}
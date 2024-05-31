using Microsoft.EntityFrameworkCore;
using ProductApi.DbContexts.ProductDb.Interfaces.Repositories;
using ProductApi.DbContexts.ProductDb.Interfaces.Seeders;
using ProductApi.DbContexts.ProductDb.Repositories;
using ProductApi.DbContexts.ProductDb.Seeders;

namespace ProductApi.DbContexts.ProductDb;

public static class ProductDb
{
    public static void AddProductDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProductDbContext>(dbContextOptions =>
            dbContextOptions.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                options => options.EnableRetryOnFailure()));

        #region Repositories

        services.AddScoped<IProductRepository, ProductRepository>();

        #endregion

        #region Seeders

        services.AddScoped<IProductSeeder, ProductSeeder>();

        #endregion
    }

    public static void ProductDbMigrate(this IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetService<ProductDbContext>();
        dbContext.Database.Migrate();

        #region Seeders

        Task.Run(async () =>
        {
            await serviceProvider.GetService<IProductSeeder>().SeedAsync();
        }).Wait();

        #endregion
    }
}
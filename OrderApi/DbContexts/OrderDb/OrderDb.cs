using Microsoft.EntityFrameworkCore;
using OrderApi.DbContexts.OrderDb.Interfaces.Repositories;
using OrderApi.DbContexts.OrderDb.Repositories;

namespace OrderApi.DbContexts.OrderDb;

public static class OrderDb
{
    public static void AddOrderDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(dbContextOptions =>
            dbContextOptions.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                options => options.EnableRetryOnFailure()));

        #region Repositories

        services.AddScoped<IOrderRepository, OrderRepository>();

        #endregion

        #region Seeders

        //services.AddScoped<IOrderSeeder, OrderSeeder>();

        #endregion
    }

    public static void OrderDbMigrate(this IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetService<OrderDbContext>();
        dbContext.Database.Migrate();

        #region Seeders

        Task.Run(async () =>
        {
            //await serviceProvider.GetService<IOrderSeeder>().SeedAsync();
        }).Wait();

        #endregion
    }
}
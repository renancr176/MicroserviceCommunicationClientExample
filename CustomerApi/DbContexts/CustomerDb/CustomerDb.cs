using CustomerApi.DbContexts.CustomerDb.Interfaces.Repositories;
using CustomerApi.DbContexts.CustomerDb.Interfaces.Seeders;
using CustomerApi.DbContexts.CustomerDb.Repositories;
using CustomerApi.DbContexts.CustomerDb.Seeders;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.DbContexts.CustomerDb;

public static class CustomerDb
{
    public static void AddCustomerDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomerDbContext>(dbContextOptions =>
            dbContextOptions.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                options => options.EnableRetryOnFailure()));

        #region Repositories

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        #endregion

        #region Seeders

        services.AddScoped<ICustomerSeeder, CustomerSeeder>();

        #endregion
    }

    public static void CustomerDbMigrate(this IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetService<CustomerDbContext>();
        dbContext.Database.Migrate();

        #region Seeders

        Task.Run(async () =>
        {
            await serviceProvider.GetService<ICustomerSeeder>().SeedAsync();
        }).Wait();

        #endregion
    }
}
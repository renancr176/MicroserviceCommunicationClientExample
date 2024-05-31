using System.Reflection;
using Identity.IdentityDbContext.Interfaces.Repositories;
using Identity.IdentityDbContext.Interfaces.Seeders;
using Identity.IdentityDbContext.Repositories;
using Identity.IdentityDbContext.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.IdentityDbContext;

public static class IdentityDb
{
    public static void AddIdentityDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(dbContextOptions =>
            dbContextOptions.UseSqlServer(configuration.GetConnectionString("IdentityConnection"),
                options =>
                {
                    options.EnableRetryOnFailure();
                    options.MigrationsAssembly(typeof(IdentityDbContext).GetTypeInfo().Assembly.GetName().Name);
                }));

        #region Repositories

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        #endregion

        #region Seeders

        services.AddScoped<IRoleSeed, RoleSeed>();

        #endregion
    }

    public static void IdentityDbMigrate(this IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetService<IdentityDbContext>();
        dbContext.Database.Migrate();

        #region Seeders

        Task.Run(async () =>
        {
            await serviceProvider.GetService<IRoleSeed>().SeedAsync();
        }).Wait();

        #endregion
    }
}
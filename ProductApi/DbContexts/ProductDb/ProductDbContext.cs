using Microsoft.EntityFrameworkCore;
using ProductApi.DbContexts.ProductDb.Entities;
using ProductApi.DbContexts.ProductDb.Mappings;

namespace ProductApi.DbContexts.ProductDb;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options)
        : base(options)
    {
    }

    #region DbSets

    public DbSet<Product> Products { get; set; }

    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        #region Mappings

        builder.ApplyConfiguration(new ProductMapping());

        #endregion
    }
}
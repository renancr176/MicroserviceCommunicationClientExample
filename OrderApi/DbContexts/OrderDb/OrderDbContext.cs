using Microsoft.EntityFrameworkCore;
using OrderApi.DbContexts.OrderDb.Entities;
using OrderApi.DbContexts.OrderDb.Mappings;

namespace OrderApi.DbContexts.OrderDb;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    #region DbSets

    public DbSet<Order> Orders { get; set; }
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

        builder.ApplyConfiguration(new OrderMapping());
        builder.ApplyConfiguration(new ProductMapping());

        #endregion
    }
}
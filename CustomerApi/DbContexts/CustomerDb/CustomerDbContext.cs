using CustomerApi.DbContexts.CustomerDb.Entities;
using CustomerApi.DbContexts.CustomerDb.Mappings;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.DbContexts.CustomerDb;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    #region DbSets

    public DbSet<Customer> Customers { get; set; }

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

        builder.ApplyConfiguration(new CustomerMapping());

        #endregion
    }
}
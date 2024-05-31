using CustomerApi.DbContexts.CustomerDb.Entities;
using DomainCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerApi.DbContexts.CustomerDb.Mappings;

public class CustomerMapping : EntityMap<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.ToTable("Customers");

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.HasIndex(e => e.Document)
            .IsUnique();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Document)
            .IsRequired()
            .HasMaxLength(50);
    }
}
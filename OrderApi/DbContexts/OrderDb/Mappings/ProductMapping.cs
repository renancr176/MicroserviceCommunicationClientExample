using DomainCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.DbContexts.OrderDb.Entities;

namespace OrderApi.DbContexts.OrderDb.Mappings;

public class ProductMapping : EntityMap<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.ToTable("Products");

        builder.HasIndex(e => new { e.Id, e.OrderId })
            .IsUnique();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Price)
            .HasPrecision(10, 2);

        #region Relationships

        builder.HasOne(e => e.Order)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.OrderId);

        #endregion
    }
}
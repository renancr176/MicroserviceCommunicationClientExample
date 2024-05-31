using DomainCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.DbContexts.OrderDb.Entities;

namespace OrderApi.DbContexts.OrderDb.Mappings;

public class OrderMapping : EntityIntIdMap<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);

        builder.ToTable("Orders");

        #region Relationships

        builder.HasMany(e => e.Products)
            .WithOne(e => e.Order)
            .HasForeignKey(e => e.OrderId);

        #endregion
    }
}
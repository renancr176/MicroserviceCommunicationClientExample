using DomainCore.Data;
using DomainCore.DomainObjects;

namespace OrderApi.DbContexts.OrderDb;

public abstract class OrderDbRepository<TEntity> : Repository<OrderDbContext, TEntity>
    where TEntity : Entity
{
    protected OrderDbRepository(OrderDbContext context) : base(context)
    {
    }
}

public abstract class OrderDbRepositoryIntId<TEntity> : RepositoryIntId<OrderDbContext, TEntity>
    where TEntity : EntityIntId
{
    protected OrderDbRepositoryIntId(OrderDbContext context) : base(context)
    {
    }
}
using DomainCore.Data;
using DomainCore.DomainObjects;

namespace ProductApi.DbContexts.ProductDb;

public abstract class ProductDbRepository<TEntity> : Repository<ProductDbContext, TEntity>
    where TEntity : Entity
{
    protected ProductDbRepository(ProductDbContext context) : base(context)
    {
    }
}

public abstract class ProductDbRepositoryIntId<TEntity> : RepositoryIntId<ProductDbContext, TEntity>
    where TEntity : EntityIntId
{
    protected ProductDbRepositoryIntId(ProductDbContext context) : base(context)
    {
    }
}
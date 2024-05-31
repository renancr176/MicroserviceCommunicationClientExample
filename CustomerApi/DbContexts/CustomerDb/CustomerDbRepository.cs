using DomainCore.Data;
using DomainCore.DomainObjects;

namespace CustomerApi.DbContexts.CustomerDb;

public abstract class CustomerDbRepository<TEntity> : Repository<CustomerDbContext, TEntity>
    where TEntity : Entity
{
    protected CustomerDbRepository(CustomerDbContext context) : base(context)
    {
    }
}

public abstract class CustomerRepositoryIntId<TEntity> : RepositoryIntId<CustomerDbContext, TEntity>
    where TEntity : EntityIntId
{
    protected CustomerRepositoryIntId(CustomerDbContext context) : base(context)
    {
    }
}

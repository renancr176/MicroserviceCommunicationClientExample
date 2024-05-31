using DomainCore.Data;
using DomainCore.DomainObjects;

namespace Identity.IdentityDbContext;

public abstract class IdentityRepository<TEntity> : Repository<IdentityDbContext, TEntity>
    where TEntity : Entity
{
    protected IdentityRepository(IdentityDbContext context)
        : base(context)
    {
    }
}

public abstract class IdentityRepositoryIntId<TEntity> : RepositoryIntId<IdentityDbContext, TEntity>
    where TEntity : EntityIntId
{
    protected IdentityRepositoryIntId(IdentityDbContext context)
        : base(context)
    {
    }
}

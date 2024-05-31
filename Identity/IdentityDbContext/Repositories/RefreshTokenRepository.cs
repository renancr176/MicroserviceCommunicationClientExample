using System.Linq.Expressions;
using Identity.IdentityDbContext.Entities;
using Identity.IdentityDbContext.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Identity.IdentityDbContext.Repositories;

public class RefreshTokenRepository : IdentityRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(IdentityDbContext context)
        : base(context)
    {
    }

    public override async Task DeleteAsync(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        DbSet.Remove(entity);
    }

    public override async Task DeleteAsync(Expression<Func<RefreshToken, bool>> predicate)
    {
        var entities = await BaseQuery.Where(predicate)
            .ToListAsync();
        DbSet.RemoveRange(entities);
    }
}
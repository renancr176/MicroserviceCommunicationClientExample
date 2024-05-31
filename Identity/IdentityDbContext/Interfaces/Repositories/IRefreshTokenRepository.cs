using DomainCore.Data;
using Identity.IdentityDbContext.Entities;

namespace Identity.IdentityDbContext.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
}
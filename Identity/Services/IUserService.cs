using DomainCore.Enums;
using Identity.IdentityDbContext.Entities;

namespace Identity.Services;

public interface IUserService
{
    public Guid? UserId { get; }
    Task<User> CurrentUserAsync();
    Task<User> FindByUserName(string userName);
    Task<bool> CurrentUserHasRole(string roleName);
    Task<bool> CurrentUserHasRole(RoleEnum role);
    Task<bool> CurrentUserHasRoleAnyAsync(Func<RoleEnum?, bool> predicate);
    Task<bool> HasRole(Guid userId, string roleName);
    Task<bool> HasRole(Guid userId, RoleEnum role);
}
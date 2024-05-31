using System.Security.Claims;
using DomainCore.Enums;
using DomainCore.Extensions;
using Identity.IdentityDbContext.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;

    public UserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    #region Consts

    public const string PasswordRole = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%¨&*_+-=^~?<>]).{8,50}$";

    public const string EmailConfirmationSubject = "Email confirmation";
    public const string EmailConfirmationBody = @"<p>Hello #Name</p>
<br/>
<p>To confirm your email, follow the above steps.</p>
<br/>
<h3 style=""text-align:center"">#EmailConfirmationToken</h3>";

    #endregion

    public Guid? UserId
    {
        get
        {

            try
            {
                if (Guid.TryParse(_httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                {
                    return id;
                }
            }
            catch (Exception e)
            {
            }

            return default;
        }
    }

    public async Task<User> CurrentUserAsync()
    {
        return UserId != null
            ? await _userManager.FindByIdAsync(UserId.ToString())
            : null;
    }

    public async Task<User> FindByUserName(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<bool> CurrentUserHasRole(string roleName)
    {
        var user = await CurrentUserAsync();
        if (user is null)
            return false;

        return _httpContextAccessor.HttpContext?.User.IsInRole(roleName) ?? false;
    }

    public async Task<bool> CurrentUserHasRole(RoleEnum role)
    {
        return await CurrentUserHasRole(role.ToString());
    }

    public async Task<bool> CurrentUserHasRoleAnyAsync(Func<RoleEnum?, bool> predicate)
    {
        var user = await CurrentUserAsync();
        if (user is null)
            return false;

        var userRoles = _httpContextAccessor.HttpContext?.User?
            .Claims?.Where(c => c.Type == ClaimTypes.Role && c.Value.ValueExistsInEnum<RoleEnum>())
            ?.Select(c => c.Value.StringToEnum<RoleEnum>())
            ?.Distinct();

        return userRoles != null && userRoles.Any(predicate);
    }

    public async Task<bool> HasRole(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<bool> HasRole(Guid userId, RoleEnum role)
    {
        return await HasRole(userId, role.ToString());
    }
}
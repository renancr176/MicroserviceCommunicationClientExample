using DomainCore.Enums;
using Identity.IdentityDbContext.Interfaces.Seeders;
using Microsoft.AspNetCore.Identity;

namespace Identity.IdentityDbContext.Seeders;

public class RoleSeed : IRoleSeed
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RoleSeed(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        foreach (var role in Enum.GetValues<RoleEnum>())
        {
            if (!await _roleManager.RoleExistsAsync(role.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role.ToString()));
            }
        }
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Services;

public static class ServicesIoC
{
    public static void AddIdentityServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
    }
}
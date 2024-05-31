namespace AuthApi.Services;

public static class ServicesIoC
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
    }
}

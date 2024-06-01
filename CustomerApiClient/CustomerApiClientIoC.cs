using CustomerApiClient.Interfaces.Services;
using CustomerApiClient.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerApiClient;

public static class CustomerApiClientIoC
{
    public static void AddCustomerApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CustomerApiClientOptions>(configuration.GetSection(CustomerApiClientOptions.sectionKey));

        #region Authentications

        services.AddSingleton<IAuthService, AuthService>();
        services.AddScoped<IForwardAuthService, ForwardAuthService>();

        #endregion

        services.AddScoped<ICustomerService, CustomerService>();
    }
}
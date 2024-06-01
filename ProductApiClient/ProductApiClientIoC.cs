using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApiClient.Interfaces.Services;
using ProductApiClient.Services;

namespace ProductApiClient;

public static class ProductApiClientIoC
{
    public static void AddProductApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ProductApiClientOptions>(configuration.GetSection(ProductApiClientOptions.sectionKey));

        #region Authentications

        services.AddSingleton<IAuthService, AuthService>();
        services.AddScoped<IForwardAuthService, ForwardAuthService>();

        #endregion

        services.AddScoped<IProductService, ProductService>();
    }
}
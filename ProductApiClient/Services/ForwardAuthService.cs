using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ProductApiClient.Interfaces.Services;

namespace ProductApiClient.Services;

public class ForwardAuthService : IForwardAuthService
{
    private readonly IOptions<ProductApiClientOptions> _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ForwardAuthService(IOptions<ProductApiClientOptions> options, IHttpContextAccessor httpContextAccessor)
    {
        _options = options;
        _httpContextAccessor = httpContextAccessor;
    }

    #region Privates

    private ProductApiClientOptions ProductApiClientOptions => _options.Value;
    private const string Bearer = "Bearer";
    private string? AccessToken => _httpContextAccessor?.HttpContext?.Request?.Headers?.Authorization.ToString()
        .Replace(Bearer, "")
        .Replace(Bearer.ToLower(), "")
        .Replace(Bearer.ToUpper(), "")
        .Trim();

    #endregion

    public bool IsUserAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public IFlurlRequest Url => ProductApiClientOptions.Url
        .WithOAuthBearerToken(AccessToken);
}
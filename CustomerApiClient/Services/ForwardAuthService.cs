using CustomerApiClient.Interfaces.Services;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CustomerApiClient.Services;

public class ForwardAuthService : IForwardAuthService
{
    private readonly IOptions<CustomerApiClientOptions> _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ForwardAuthService(IOptions<CustomerApiClientOptions> options, IHttpContextAccessor httpContextAccessor)
    {
        _options = options;
        _httpContextAccessor = httpContextAccessor;
    }

    #region Privates

    private CustomerApiClientOptions CustomerApiClientOptions => _options.Value;
    private const string Bearer = "Bearer";
    private string? AccessToken => _httpContextAccessor?.HttpContext?.Request?.Headers?.Authorization.ToString()
        .Replace(Bearer, "")
        .Replace(Bearer.ToLower(), "")
        .Replace(Bearer.ToUpper(), "")
        .Trim();

    #endregion

    public bool IsUserAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public IFlurlRequest Url => CustomerApiClientOptions.Url
        .WithOAuthBearerToken(AccessToken);
}
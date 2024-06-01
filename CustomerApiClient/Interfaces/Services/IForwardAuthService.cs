using Flurl.Http;

namespace CustomerApiClient.Interfaces.Services;

public interface IForwardAuthService
{
    public bool IsUserAuthenticated { get; }
    IFlurlRequest Url { get; }
}
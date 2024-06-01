using Flurl.Http;

namespace ProductApiClient.Interfaces.Services;

public interface IForwardAuthService
{
    public bool IsUserAuthenticated { get; }
    IFlurlRequest Url { get; }
}
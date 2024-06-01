using Flurl.Http;

namespace CustomerApiClient.Interfaces.Services;

public interface IAuthService
{
    IFlurlRequest Url { get; }
}
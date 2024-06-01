using Flurl.Http;

namespace ProductApiClient.Interfaces.Services;

public interface IAuthService
{
    IFlurlRequest Url { get; }
}
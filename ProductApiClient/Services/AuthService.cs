using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProductApiClient.Interfaces.Services;
using ProductApiClient.Models.Responses;

namespace ProductApiClient.Services;

public class AuthService : IAuthService
{
    private readonly IOptions<ProductApiClientOptions> _options;

    public AuthService(IOptions<ProductApiClientOptions> options)
    {
        _options = options;
    }

    #region Privates

    private ProductApiClientOptions ProductApiClientOptions => _options.Value;
    private DateTime TokenValidUntil { get; set; } = DateTime.UtcNow;
    private SignInResponse SignInResponse { get; set; }

    private string GetAccessToken()
    {
        if (TokenValidUntil > DateTime.UtcNow && !string.IsNullOrEmpty(SignInResponse?.AccessToken))
            return SignInResponse.AccessToken;

        try
        {
            Task.Run(async () =>
            {
                try
                {
                    IFlurlResponse result;
                    if (string.IsNullOrEmpty(SignInResponse?.AccessToken) || string.IsNullOrEmpty(SignInResponse?.RefreshToken))
                    {
                        result = await ProductApiClientOptions.AuthUrl
                            .PostJsonAsync(new
                            {
                                username = ProductApiClientOptions.UserName,
                                password = ProductApiClientOptions.Password,
                            });
                    }
                    else
                    {
                        result = await ProductApiClientOptions.AuthUrl
                            .AppendPathSegment("/Refresh")
                            .PostJsonAsync(new
                            {
                                SignInResponse.AccessToken,
                                SignInResponse.RefreshToken
                            });
                    }

                    SignInResponse = JsonConvert.DeserializeObject<SignInResponse>(await result.GetStringAsync());

                    TokenValidUntil = DateTime.UtcNow.Add(TimeSpan.FromSeconds(SignInResponse.ExpiresInSeconds));
                }
                catch (Exception e) { }
            }).Wait();
        }
        catch (Exception e)
        {
        }

        return SignInResponse?.AccessToken;
    }

    #endregion

    public IFlurlRequest Url => ProductApiClientOptions.Url
        .WithOAuthBearerToken(GetAccessToken());
}
using CustomerApiClient.Interfaces.Services;
using CustomerApiClient.Models.Responses;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CustomerApiClient.Services;

public class AuthService : IAuthService
{
    private readonly IOptions<CustomerApiClientOptions> _options;

    public AuthService(IOptions<CustomerApiClientOptions> options)
    {
        _options = options;
    }

    #region Privates

    private CustomerApiClientOptions CustomerApiClientOptions => _options.Value;
    private DateTime TokenValidUntil { get; set; } = DateTime.UtcNow;
    private DateTime RefreshTokenValidUntil { get; set; } = DateTime.UtcNow;
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
                    if (!string.IsNullOrEmpty(SignInResponse?.AccessToken) 
                    && !string.IsNullOrEmpty(SignInResponse?.RefreshToken)
                    && RefreshTokenValidUntil > DateTime.UtcNow)
                    {
                        result = await CustomerApiClientOptions.AuthUrl
                            .AppendPathSegment("/Refresh")
                            .PostJsonAsync(new
                            {
                                SignInResponse.AccessToken,
                                SignInResponse.RefreshToken
                            });
                    }
                    else
                    {
                        result = await CustomerApiClientOptions.AuthUrl
                            .PostJsonAsync(new
                            {
                                username = CustomerApiClientOptions.UserName,
                                password = CustomerApiClientOptions.Password,
                            });
                    }

                    SignInResponse = JsonConvert.DeserializeObject<SignInResponse>(await result.GetStringAsync());

                    TokenValidUntil = DateTime.UtcNow.Add(TimeSpan.FromSeconds(SignInResponse.ExpiresInSeconds));
                    RefreshTokenValidUntil = DateTime.UtcNow.Add(TimeSpan.FromSeconds(SignInResponse.RefreshTokenExpiresInSeconds));
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

    public IFlurlRequest Url => CustomerApiClientOptions.Url
        .WithOAuthBearerToken(GetAccessToken());
}
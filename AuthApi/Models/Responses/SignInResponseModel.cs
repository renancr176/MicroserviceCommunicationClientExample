namespace AuthApi.Models.Responses;

public class SignInResponseModel
{
    public string AccessToken { get; set; }
    public double ExpiresInSeconds { get; set; }
    public string RefreshToken { get; set; }
    public double RefreshTokenExpiresInSeconds { get; set; }
    public UserModel User { get; set; }

    public SignInResponseModel()
    {
    }

    public SignInResponseModel(string accessToken, double expiresInSeconds, string refreshToken, double refreshTokenExpiresInSeconds, UserModel user)
    {
        AccessToken = accessToken;
        ExpiresInSeconds = expiresInSeconds;
        RefreshToken = refreshToken;
        RefreshTokenExpiresInSeconds = refreshTokenExpiresInSeconds;
        User = user;
    }
}
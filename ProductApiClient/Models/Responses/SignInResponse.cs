namespace ProductApiClient.Models.Responses;

public class SignInResponse
{
    public string AccessToken { get; set; }
    public double ExpiresInSeconds { get; set; }
    public string RefreshToken { get; set; }
    public double RefreshTokenExpiresInSeconds { get; set; }
}
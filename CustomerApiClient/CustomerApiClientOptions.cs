namespace CustomerApiClient;

public class CustomerApiClientOptions
{
    public static string sectionKey => "CustomerApi";

    public string AuthUrl { get; set; }
    public string Url { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }

    public void Ok()
    {
        if (string.IsNullOrEmpty(AuthUrl)
            || !Uri.IsWellFormedUriString(AuthUrl, UriKind.Absolute))
            throw new ArgumentNullException(nameof(AuthUrl), $"The {nameof(AuthUrl)} API was not properly configured.");

        if (string.IsNullOrEmpty(Url)
            || !Uri.IsWellFormedUriString(Url, UriKind.Absolute))
            throw new ArgumentNullException(nameof(Url), $"The {nameof(Url)} of customer's API was not properly configured.");

        if (string.IsNullOrEmpty(UserName))
            throw new ArgumentNullException(nameof(UserName), $"The {nameof(UserName)} of customer's API was not properly configured.");

        if (string.IsNullOrEmpty(Password))
            throw new ArgumentNullException(nameof(Password), $"The {nameof(Password)} of customer's API was not properly configured.");
    }
}
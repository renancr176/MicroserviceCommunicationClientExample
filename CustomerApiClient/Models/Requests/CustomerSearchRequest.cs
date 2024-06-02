namespace CustomerApiClient.Models.Requests;

//[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
//[JsonObject(NamingStrategyType = typeof(KebabCaseNamingStrategy))]
//[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class CustomerSearchRequest
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 50;
    public Guid? UserId { get; set; }
    public string? Name { get; set; }
    public string? Document { get; set; }

    public CustomerSearchRequest(int page = 1, int size = 50, Guid? userId = null, string? name = null, string? document = null)
    {
        Page = page;
        Size = size;
        UserId = userId;
        Name = name;
        Document = document;
    }
}
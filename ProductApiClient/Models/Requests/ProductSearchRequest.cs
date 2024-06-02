using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ProductApiClient.Models.Requests;

//[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
//[JsonObject(NamingStrategyType = typeof(KebabCaseNamingStrategy))]
//[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class ProductSearchRequest
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 50;
    public IEnumerable<Guid>? Ids { get; set; } = new List<Guid>();
    public string? Name { get; set; }
    public bool? Active { get; set; }

    public ProductSearchRequest(int page = 1, int size = 50, IEnumerable<Guid>? ids = null, string? name = null, bool? active = null)
    {
        Page = page;
        Size = size;
        Ids = ids ?? new List<Guid>();
        Name = name;
        Active = active;
    }
}
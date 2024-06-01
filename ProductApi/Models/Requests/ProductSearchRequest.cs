namespace ProductApi.Models.Requests;

public class ProductSearchRequest
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 50;
    public IEnumerable<Guid>? Ids { get; set; } = new List<Guid>();
    public string? Name { get; set; }
    public bool? Active { get; set; }
}
namespace CustomerApi.Models.Requests;

public class CustomerSearchRequest
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 50;
    public Guid? UserId { get; set; }
    public string? Name { get; set; }
    public string? Document { get; set; }
}
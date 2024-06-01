namespace ProductApiClient.Models;

public class ProductResponseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool Active { get; set; }
}
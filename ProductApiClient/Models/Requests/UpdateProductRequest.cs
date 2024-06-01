namespace ProductApiClient.Models.Requests;

public class UpdateProductRequest : CreateProductRequest
{
    public Guid Id { get; set; }
}
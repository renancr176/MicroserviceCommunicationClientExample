using System.ComponentModel.DataAnnotations;

namespace ProductApiClient.Models.Requests;

public class CreateProductRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool Active { get; set; } = true;
}
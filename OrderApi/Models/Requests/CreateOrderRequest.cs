using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models.Requests;

public class CreateOrderRequest
{
    [Required, MinLength(1)]
    public IEnumerable<ProductRequest> Products { get; set; }
}
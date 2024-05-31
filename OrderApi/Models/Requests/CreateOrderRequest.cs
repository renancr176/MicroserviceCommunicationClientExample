using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models.Requests;

public class CreateOrderRequest
{
    [Required]
    [Range(1, Int32.MaxValue)]
    public IEnumerable<ProductRequest> IdProducts { get; set; }
}
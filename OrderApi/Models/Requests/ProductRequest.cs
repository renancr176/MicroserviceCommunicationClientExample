using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models.Requests;

public class ProductRequest
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    [Range(0, Int32.MaxValue)]
    public int Quantity { get; set; } = 0;
}
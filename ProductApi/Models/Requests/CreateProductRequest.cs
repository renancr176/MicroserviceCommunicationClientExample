using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models.Requests;

public class CreateProductRequest
{
    [Required]
    [MinLength(3)]
    public string Name { get; set; }
    [Required]
    [Range(typeof(decimal), "0", "9999.99")]
    public decimal Price { get; set; }
    public bool Active { get; set; } = true;
}
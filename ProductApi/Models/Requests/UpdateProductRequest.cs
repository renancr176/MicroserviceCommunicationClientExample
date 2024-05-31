using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models.Requests;

public class UpdateProductRequest : CreateProductRequest
{
    [Required]
    public Guid Id { get; set; }
}
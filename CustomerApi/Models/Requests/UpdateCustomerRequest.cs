using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models.Requests;

public class UpdateCustomerRequest : CreateCustomerRequest
{
    [Required]
    public Guid Id { get; set; }
}
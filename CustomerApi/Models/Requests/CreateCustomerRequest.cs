using CustomerApi.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models.Requests;

public class CreateCustomerRequest
{
    [Required]
    [MinLength(3)]
    public string Name { get; set; }
    [Required]
    [MinDateTime("1920-01-01")]
    public DateTime BirthDate { get; set; }
    [Required]
    [MinLength(3)]
    public string Document { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
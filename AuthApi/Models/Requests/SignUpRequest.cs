using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.Requests;

public class SignUpRequest
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string RememberPhrase { get; set; }
}
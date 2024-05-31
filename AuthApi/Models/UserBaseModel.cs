using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models;

public class UserBaseModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Name { get; set; }
}
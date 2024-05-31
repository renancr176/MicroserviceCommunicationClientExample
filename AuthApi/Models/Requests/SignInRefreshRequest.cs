using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.Requests;

public class SignInRefreshRequest
{
    [Required]
    public string AccessToken { get; set; }
    [Required]
    public string RefreshToken { get; set; }
}
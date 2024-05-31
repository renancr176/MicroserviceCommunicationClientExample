using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.Requests;

public class SignInRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
}
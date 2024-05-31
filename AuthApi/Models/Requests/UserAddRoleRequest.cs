using System.ComponentModel.DataAnnotations;
using DomainCore.Enums;
using System.Text.Json.Serialization;

namespace AuthApi.Models.Requests;

public class UserAddRoleRequest
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IEnumerable<RoleEnum> Roles { get; set; }
}
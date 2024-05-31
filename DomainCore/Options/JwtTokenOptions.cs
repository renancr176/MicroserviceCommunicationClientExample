using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DomainCore.Options;

public class JwtTokenOptions
{
    public static string sectionKey = "Jwt";
    public string SecretKey { get; set; }

    public string Issuer { get; set; }

    public string Subject { get; set; }

    public string Audience { get; set; }

    public int ValidForMinutes { get; set; } = 120;

    public DateTime NotBefore => DateTime.UtcNow;

    public DateTime IssuedAt => DateTime.UtcNow;

    public TimeSpan ValidFor => TimeSpan.FromMinutes(ValidForMinutes);

    public DateTime Expiration => IssuedAt.Add(ValidFor);

    public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());

    public SymmetricSecurityKey IssuerSigningKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
    public string JwtSecurityAlgorithms => SecurityAlgorithms.HmacSha256;
    public SigningCredentials SigningCredentials => new SigningCredentials(IssuerSigningKey, JwtSecurityAlgorithms);
    public int RefreshTokenValidForMoreMinutes { get; set; }
    public TimeSpan RefreshTokenValidForMore => TimeSpan.FromMinutes(RefreshTokenValidForMoreMinutes > 0
        ? RefreshTokenValidForMoreMinutes
        : ValidForMinutes * 2);
    public DateTime RefreshTokenValidUntil => Expiration.Add(RefreshTokenValidForMore);
}

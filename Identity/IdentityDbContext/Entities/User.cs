using Microsoft.AspNetCore.Identity;

namespace Identity.IdentityDbContext.Entities;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; }
    public string RememberPhrase { get; set; }
    public string? EmailConfirmationToken { get; set; }
    public long UsedBytes { get; set; } = 0;
    public string? ResetPasswordToken { get; set; }

    #region Relationships

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

    #endregion

    public User(string userName, string name, string email, string rememberPhrase)
        : base(userName)
    {
        Name = name;
        Email = email;
        RememberPhrase = rememberPhrase;
        EmailConfirmed = true;
        SetEmailConfirmationToken();
    }

    public void SetEmailConfirmationToken(string emailConfirmationToken = null)
    {
        EmailConfirmationToken = emailConfirmationToken ?? new Random().Next(0, 1000000).ToString("D6");
    }
}
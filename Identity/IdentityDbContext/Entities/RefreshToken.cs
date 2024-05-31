using DomainCore.DomainObjects;

namespace Identity.IdentityDbContext.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ValidUntil { get; set; }

    #region Relationships

    public User User { get; set; }

    #endregion

    public RefreshToken()
    {
        ValidUntil = DateTime.UtcNow.AddDays(1);
    }

    public RefreshToken(Guid userId, string token)
        : this()
    {
        UserId = userId;
        Token = token;
    }

    public RefreshToken(Guid userId, string token, DateTime validUntil)
        : this(userId, token)
    {
        ValidUntil = validUntil;
    }
}
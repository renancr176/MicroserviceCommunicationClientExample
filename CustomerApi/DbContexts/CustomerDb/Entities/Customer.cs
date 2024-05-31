using DomainCore.DomainObjects;

namespace CustomerApi.DbContexts.CustomerDb.Entities;

public class Customer : Entity
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }

    public Customer(Guid userId, string name, DateTime birthDate, string document, string email)
    {
        UserId = userId;
        Name = name;
        BirthDate = birthDate;
        Document = document;
        Email = email;
    }
}
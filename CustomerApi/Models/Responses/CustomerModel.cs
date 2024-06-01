namespace CustomerApi.Models.Responses;

public class CustomerModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
}
namespace CustomerApiClient.Models;

public class CustomerResponseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
}
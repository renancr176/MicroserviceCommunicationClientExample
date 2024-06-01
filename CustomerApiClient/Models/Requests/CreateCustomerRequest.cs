namespace CustomerApiClient.Models.Requests;

public class CreateCustomerRequest
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }

    public CreateCustomerRequest(string name, DateTime birthDate, string document, string email)
    {
        Name = name;
        BirthDate = birthDate;
        Document = document;
        Email = email;
    }
}
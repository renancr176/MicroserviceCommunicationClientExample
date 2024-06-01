namespace CustomerApiClient.Models.Requests;

public class UpdateCustomerRequest : CreateCustomerRequest
{
    public Guid Id { get; set; }

    public UpdateCustomerRequest(string name, DateTime birthDate, string document, string email, Guid id) 
        : base(name, birthDate, document, email)
    {
        Id = id;
    }
}
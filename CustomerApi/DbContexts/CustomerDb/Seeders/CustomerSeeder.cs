using Bogus;
using Bogus.Extensions.UnitedStates;
using CustomerApi.DbContexts.CustomerDb.Entities;
using CustomerApi.DbContexts.CustomerDb.Interfaces.Repositories;
using CustomerApi.DbContexts.CustomerDb.Interfaces.Seeders;
using Identity.IdentityDbContext.Entities;
using Microsoft.AspNetCore.Identity;

namespace CustomerApi.DbContexts.CustomerDb.Seeders;

public class CustomerSeeder : ICustomerSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly ICustomerRepository _customerRepository;

    public CustomerSeeder(UserManager<User> userManager, ICustomerRepository customerRepository)
    {
        _userManager = userManager;
        _customerRepository = customerRepository;
    }

    public async Task SeedAsync()
    {
        if (!await _customerRepository.AnyAsync(c => true))
        {
            for (int i = 0; i < 100; i++)
            {
                User user;
                var password = "";
                Customer customer;

                do
                {
                    var faker = new Faker();
                    password = faker.Internet.Password(8, false, "[A-Za-z\\d]", "Ab@1");
                    user = new User(faker.Person.Email.ToLower(), faker.Person.FullName, faker.Person.Email.ToLower(),
                        password);
                    customer = new Customer(user.Id, user.Name, faker.Person.DateOfBirth, faker.Person.Ssn(),
                        user.Email);
                } while ((await _userManager.FindByNameAsync(user.UserName)) != null
                         || await _customerRepository.AnyAsync(c =>
                             c.Document == customer.Document || c.Email == customer.Email));

                await _userManager.CreateAsync(user, password);
                customer.UserId = user.Id;
                await _customerRepository.InsertAsync(customer);
                await _customerRepository.SaveChangesAsync();
            }
        }
    }
}
using CustomerApi.DbContexts.CustomerDb.Entities;
using CustomerApi.DbContexts.CustomerDb.Interfaces.Repositories;

namespace CustomerApi.DbContexts.CustomerDb.Repositories;

public class CustomerRepository : CustomerDbRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(CustomerDbContext context) : base(context)
    {
    }
}
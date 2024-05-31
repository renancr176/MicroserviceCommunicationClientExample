using CustomerApi.DbContexts.CustomerDb.Entities;
using DomainCore.Data;

namespace CustomerApi.DbContexts.CustomerDb.Interfaces.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
}
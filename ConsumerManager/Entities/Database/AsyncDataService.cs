using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace ConsumerManager.Entities.Database
{
  public class AsyncDataService : IDataServiceProvider
  {
    private readonly RelationalModel model;
    public AsyncDataService(RelationalModel model)
    {
      this.model = model;
    }

    public virtual async Task<List<Customer>> GetAllCustomers()
    {
      var customers = await model.Customers.ToListAsync();
      return customers;
    }

    public virtual async Task<Customer?> GetCustomerById(int id)
    {
      var customer = await model.Customers.FindAsync(id);
      return customer;
    }

    public virtual async Task<bool> CustomerExists(string email, string phone)
    {
      var customer = await model.Customers.FirstOrDefaultAsync(
        customer => customer.Email == email.ToLower()
        || customer.Phone == phone
      );
      return customer is not null;
    }

    public virtual async Task CreateCustomer(Customer customer)
    {
      await model.Customers.AddAsync(customer);
      await model.SaveChangesAsync();
    }
  }
}

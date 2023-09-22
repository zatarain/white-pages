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
      var customers = await model.Customers
        .Include(customer => customer.Addresses).ToListAsync();
      return customers;
    }

    public virtual async Task<List<Customer>> GetOnlyActiveCustomers()
    {
      var customers = await model.Customers
        .Include(customer => customer.Addresses)
        .Where(customer => customer.IsActive).ToListAsync();
      return customers;
    }

    public virtual async Task<Customer?> GetCustomerById(int id)
    {
      var customer = await model.Customers
        .Include(customer => customer.Addresses)
        .FirstOrDefaultAsync(customer => customer.Id == id);
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

    public virtual async Task DeleteCustomer(Customer customer)
    {
      model.Customers.Remove(customer);
      await model.SaveChangesAsync();
    }

    public virtual Task UpdateCustomer(Customer customer)
    {
      throw new NotImplementedException();
    }

    public virtual async Task<Customer?> UpdateCustomerStatus(int id, bool isActive)
    {
      var customer = await GetCustomerById(id);
      if (customer is not null)
      {
        customer.IsActive = isActive;
        await model.SaveChangesAsync();
      }
      return customer;
    }

    public virtual async Task<Customer?> GetCustomerByAddressId(int id)
    {
      var address = await model.Addresses
        .FirstOrDefaultAsync(address => address.Id == id);
      var customer = await GetCustomerById(address?.CustomerId ?? 0);
      return customer;
    }

    public virtual async Task CreateAddress(Address address)
    {
      await model.Addresses.AddAsync(address);
      await model.SaveChangesAsync();
    }

    public virtual async Task DeleteAddress(Address address)
    {
      model.Addresses.Remove(address);
      await model.SaveChangesAsync();
    }
  }
}

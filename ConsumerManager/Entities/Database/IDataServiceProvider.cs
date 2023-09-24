namespace ConsumerManager.Entities.Database
{
  public interface IDataServiceProvider
  {
    public Task CreateCustomer(Customer customer);
    public Task<bool> CustomerExists(string email, string phone);
    public Task<List<Customer>> GetAllCustomers();
    public Task<Customer?> GetCustomerById(int id);
    public Task<List<Customer>> GetOnlyActiveCustomers();
    public Task DeleteCustomer(Customer customer);
    public Task UpdateCustomer(Customer customer);
    public Task<Customer?> UpdateCustomerStatus(int id, bool isActive);
    public Task<Customer?> GetCustomerByAddressId(int id);
    public Task CreateAddress(Address address);    
    public Task DeleteAddress(Address address);
  }
}
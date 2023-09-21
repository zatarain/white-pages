namespace ConsumerManager.Entities.Database
{
  public interface IDataServiceProvider
  {
    Task CreateCustomer(Customer customer);
    Task<bool> CustomerExists(string email, string phone);
    Task<List<Customer>> GetAllCustomers();
    Task<Customer?> GetCustomerById(int id);
  }
}
using ConsumerManager.Entities;
using ConsumerManager.Entities.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ConsumerManager.Controllers
{
  [ApiController]
  [Route("customers")]
  public class CustomersController : ControllerBase
  {
    private readonly ILogger<CustomersController> logger;

    private readonly RelationalModel model;

    public CustomersController(ILogger<CustomersController> logger, RelationalModel model)
    {
      this.logger = logger;
      this.model = model;
    }

    [HttpGet] // GET /customers
    public async Task<ActionResult<List<Customer>>> ReadAll()
    {
      logger.LogInformation("Getting list of customers");
      return await Task.FromResult(model.Customers.ToList());
    }

    [HttpGet("{id}")] // GET /customers/{id}
    public async Task<ActionResult<Customer>> Read(int id)
    {
      logger.LogInformation("Getting a single customer");
      var customer = await model.Customers.FindAsync(id);
      if (customer is null)
      {
        return NotFound();
      }
      return customer;
    }
    [HttpPost]
    public async Task<ActionResult<Customer>> Create(CreateCustomerContract contract)
    {
      // model.AddAsync(contract);
      var customer = new Customer
      {
        Title = contract.Title,
        Forename = contract.Forename,
        Surename = contract.Surename,
        Email = contract.Email,
        Phone = contract.Phone,
        CreatedAt = DateTime.UtcNow,
        LastUpdatedAt = DateTime.UtcNow,
      };
      await model.AddAsync(customer);
      await model.SaveChangesAsync();
      logger.LogInformation($"Created new customer with Id = {customer.Id}");
      return CreatedAtAction(nameof(Read), new { id = customer.Id }, customer);
    }
  }
}

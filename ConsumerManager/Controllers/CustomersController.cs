using ConsumerManager.Entities;
using ConsumerManager.Entities.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ConsumerManager.Controllers
{
  [ApiController]
  [Route("customers")]
  public class CustomersController : ControllerBase
  {
    private readonly ILogger<CustomersController> logger;

    private readonly IDataServiceProvider service;

    public CustomersController(ILogger<CustomersController> logger, IDataServiceProvider service)
    {
      this.logger = logger;
      this.service = service;
    }

    [HttpGet] // GET /customers
    public async Task<ActionResult<List<Customer>>> ReadAll()
    {
      logger.LogInformation("Getting list of customers");
      return await service.GetAllCustomers();
    }

    [HttpGet("{id}")] // GET /customers/{id}
    public async Task<ActionResult<Customer>> Read(int id)
    {
      logger.LogInformation("Getting a single customer");
      var customer = await service.GetCustomerById(id);
      if (customer is null)
      {
        return NotFound();
      }
      return customer;
    }
    [HttpPost]
    public async Task<ActionResult<Customer>> Create([FromBody] CreateCustomerContract? contract)
    {
      logger.LogInformation("Trying to create a new customer");
      if (contract is null)
      {
        return BadRequest("You need to provide data for new customer.");
      }

      if (await service.CustomerExists(contract.Email, contract.Phone))
      {
        return BadRequest($"A customer with the email '{contract.Email}' and/or phone '{contract.Phone}' already exists.");
      }

      var customer = new Customer
      {
        Title = contract.Title,
        Forename = contract.Forename,
        Surname = contract.Surname,
        Email = contract.Email.ToLower(),
        Phone = contract.Phone,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        LastUpdatedAt = DateTime.UtcNow,
      };

      logger.LogInformation("New customer created successfully.");
      await service.CreateCustomer( customer );
      return CreatedAtAction(nameof(Read), new { id = customer.Id }, customer);
    }
  }
}

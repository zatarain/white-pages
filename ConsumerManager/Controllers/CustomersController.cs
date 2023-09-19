using ConsumerManager.Entities;
using ConsumerManager.Entities.Database;
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
    public ActionResult<List<Customer>> ReadAll()
    {
      logger.LogInformation("Getting list of customers");
      return model.Customers.ToList();
    }

    [HttpGet("{id}")] // GET /customers/{id}
    public ActionResult<Customer> Read(int id)
    {
      logger.LogInformation("Getting a single customer");
      var customer = model.Customers.Find(id);
      if (customer is null)
      {
        return NotFound();
      }
      return customer;
    }
  }
}

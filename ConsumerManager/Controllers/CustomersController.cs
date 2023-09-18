using ConsumerManager.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ConsumerManager.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class CustomersController : ControllerBase
  {
    private readonly ILogger<CustomersController> logger;

    public CustomersController(ILogger<CustomersController> logger)
    {
      this.logger = logger;
    }

    [HttpGet(Name = "CustomerList")]
    public IEnumerable<Customer> Index()
    {
      logger.LogInformation("Getting list of customers");
      return Enumerable.Empty<Customer>();
    }
  }
}

using ConsumerManager.Entities;
using ConsumerManager.Entities.Database;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ConsumerManager.Controllers
{
  [ApiController]
  [Route("addresses")]
  public class AddressesController : ControllerBase
  {
    private readonly ILogger<CustomersController> logger;

    private readonly IDataServiceProvider service;

    public AddressesController(ILogger<CustomersController> logger, IDataServiceProvider service)
    {
      this.logger = logger;
      this.service = service;
    }

    [HttpGet("{id}")] // GET /addresses/{id}
    public async Task<ActionResult<Customer>> Read(int id)
    {
      var customer = await service.GetCustomerByAddressId(id);
      if (customer is null)
      {
        return NotFound();
      }
      return Ok(customer);
    }

    [HttpPost("{customerId}")] // POST /addresses
    public async Task<ActionResult<Address>> Create(int customerId, [FromBody] CreateAddressRequest request)
    {
      logger.LogInformation("Trying to create a new customer");
      if (request is null)
      {
        return BadRequest("You need to provide data for new address.");
      }

      var customer = await service.GetCustomerById(customerId);
      if (customer is null)
      {
        return NotFound("Customer doesn't exists!");
      }

      var now = DateTime.UtcNow;
      var address = new Address
      {
        CustomerId = customerId,
        Line1 = request.Line1,
        Line2 = request.Line2 ?? "",
        Town = request.Town,
        County = request.County ?? "",
        Postcode = request.Postcode,
        Country = request.Country ?? "GB",
        CreatedAt = now,
        LastUpdatedAt = now,
      };

      await service.CreateAddress(address);
      logger.LogInformation("New address created successfully.");
      return CreatedAtAction(nameof(Read), new { id = address.Id }, address);
    }

    [HttpDelete("{id}")] // DELETE /addresses/5
    public async Task<ActionResult> Delete(int id)
    {
      logger.LogInformation("Deleting address.");
      var customer = await service.GetCustomerByAddressId(id);
      if (customer is null)
      {
        return NotFound();
      }

      if (customer.Addresses?.Count <= 1)
      {
        logger.LogInformation("Customer dons't have enough addresses to delete.");
        return BadRequest("Cannot delete the last address of the customer.");
      }

      var address = customer.Addresses?.First(address => address.Id == id);
      if (address is null)
      {
        return NotFound();
      }
      await service.DeleteAddress(address);

      if (customer.MainAddressId == id)
      {
        var addressId = customer.Addresses?.FirstOrDefault(address => address.Id != id)?.Id;
        Customer updated = customer with { MainAddressId = addressId ?? 0 };
        await service.UpdateCustomer(updated);
      }

      logger.LogInformation("Address successfully deleted.");
      return NoContent();
    }
  }
}

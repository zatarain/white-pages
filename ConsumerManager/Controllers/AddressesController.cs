﻿using ConsumerManager.Entities;
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

    [HttpPost] // POST /addresses
    public async Task<ActionResult<Address>> Create([FromBody] CreateAddressRequest request)
    {
      logger.LogInformation("Trying to create a new customer");
      if (request is null)
      {
        return BadRequest("You need to provide data for new address.");
      }

      var customer = await service.GetCustomerById(request.CustomerId ?? 0);
      if (customer is null) 
      {
        return NotFound("Customer doesn't exists!");
      }

      var now = DateTime.UtcNow;
      var address = new Address
      {
        CustomerId = customer.Id,
        Line1 = request.Line1,
        Line2 = request.Line2,
        Town = request.Town,
        County = request.County,
        Postcode = request.Postcode,
        Country = request.Country,
        CreatedAt = now,
        LastUpdatedAt = now,
      };

      await service.CreateAddress(address);
      logger.LogInformation("New address created successfully.");
      return CreatedAtAction("/customers", new { id = customer.Id }, customer);
    }

    [HttpDelete("{id}")] // DELETE /addresses/5
    public void Delete(int id)
    {
    }
  }
}
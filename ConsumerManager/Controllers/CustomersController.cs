﻿using ConsumerManager.Entities;
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
      var customers = await service.GetAllCustomers();
      return Ok(customers);
    }

    [HttpGet("only-active")] // GET /customers/only-active
    public async Task<ActionResult<List<Customer>>> GetActive()
    {
      logger.LogInformation("Getting list of active customers");
      var customers = await service.GetOnlyActiveCustomers();
      return Ok(customers);
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
      return Ok(customer);
    }
    
    [HttpPost] // POST /customers
    public async Task<ActionResult<Customer>> Create([FromBody] CreateCustomerRequest request)
    {
      logger.LogInformation("Trying to create a new customer");
      
      if (await service.CustomerExists(request.Email, request.Phone))
      {
        return BadRequest($"A customer with the email '{request.Email}' and/or phone '{request.Phone}' already exists.");
      }

      var now = DateTime.UtcNow;
      var customer = new Customer
      {
        Id = 0,
        Title = request.Title,
        Forename = request.Forename,
        Surname = request.Surname,
        Email = request.Email.ToLower(),
        Phone = request.Phone,
        IsActive = true,
        MainAddressId = 0,
        CreatedAt = now,
        LastUpdatedAt = now,
      };

      await service.CreateCustomer( customer );
      logger.LogInformation("New customer created successfully.");
      return CreatedAtAction(nameof(Read), new { id = customer.Id }, customer);
    }

    [HttpDelete("{id}")] // DELETE /customers/{id}
    public async Task<ActionResult> Delete(int id)
    {
      logger.LogInformation("Deleting customer");
      var customer = await service.GetCustomerById(id);
      if (customer is null)
      {
        return NotFound();
      }
      
      await service.DeleteCustomer(customer);
      logger.LogInformation("Customer was removed successfully from database.");
      return NoContent();
    }

    [HttpPatch("{id}/deactivate")] // PATCH /customers/{id}/deactivate
    public async Task<ActionResult<Customer>> Deactivate(int id)
    {
      logger.LogInformation("Deactivating customer");
      var customer = await service.UpdateCustomerStatus(id, false);
      if (customer is null)
      {
        return NotFound();
      }

      logger.LogInformation("Customer was successfully deactivated.");
      return Ok(customer);
    }

    [HttpPatch("{id}/activate")] // PATCH /customers/{id}/activate
    public async Task<ActionResult<Customer>> Activate(int id)
    {
      logger.LogInformation("Activating customer");
      var customer = await service.UpdateCustomerStatus(id, true);
      if (customer is null)
      {
        return NotFound();
      }

      logger.LogInformation("Customer was successfully activated.");
      return Ok(customer);
    }
  }
}

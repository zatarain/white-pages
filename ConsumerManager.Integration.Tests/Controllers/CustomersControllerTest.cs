using ConsumerManager.Controllers;
using ConsumerManager.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerManager.Integration.Tests.Controllers
{
  public class CustomersControllerTest : IClassFixture<TestApplicationFactory<Program>>
  {
    private readonly TestApplicationFactory<Program> factory;

    public CustomersControllerTest(TestApplicationFactory<Program> factory)
    {
      this.factory = factory;
    }

    [Theory]
    [InlineData("Mr.", "John", "Smith", "john.smith@example.com", "+4407111222333")]
    [InlineData("Mr.", "Another", "Customer", "another.customer@example.com", "07444555666")]

    public async Task Create_WithValidData_ReturnsCreatedCustomer(string title, string forename, string surname, string email, string phone)
    {
      // Arrange
      var client = factory.CreateClient();

      CreateCustomerRequest request = new()
      {
        Title = title,
        Forename = forename,
        Surname = surname,
        Email = email,
        Phone = phone,
      };

      var body = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8)
      {
        Headers = {
          ContentType = new MediaTypeHeaderValue("application/json"),
        }
      };

      // Act
      var response = await client.PostAsync("/customers", body);

      // Assert
      response.StatusCode.Should().Be(HttpStatusCode.Created);
      var customer = await response.Content.ReadFromJsonAsync<Customer>();
      request.Should().BeEquivalentTo(
        customer,
        options => options.ComparingByMembers<Customer>().ExcludingMissingMembers()
      );
    }

    [Theory]
    [InlineData(null, "New", "Customer", "john.smith@example.com", "+4407111222333")]
    [InlineData("Mr.", null, "Customer", "john.smith@example.com", "+4407111222333")]
    [InlineData("Mr.", "New", null, "john.smith@example.com", "+4407111222333")]
    [InlineData("Mr.", "New", "Customer", null, "+4407111222333")]
    [InlineData("Mr.", "New", "Customer", "john.smith@example.com", null)]
    [InlineData("", "New", "Customer", "john.smith@example.com", "+4407111222333")]
    [InlineData("Mr.", "", "Customer", "john.smith@example.com", "+4407111222333")]
    [InlineData("Mr.", "New", "", "john.smith@example.com", "+4407111222333")]
    [InlineData("Mr.", "New", "Customer", "", "+4407111222333")]
    [InlineData("Mr.", "New", "Customer", "john.smith@example.com", "")]
    [InlineData("Mr.", "New", "Customer", "@john.smith", "+4407111222333")]
    [InlineData("Mr.", "New", "Customer", "john.smith@example.com", "a-phone-number")]
    public async Task Create_WithInvalidData_ReturnsBadRequest(string title, string forename, string surname, string email, string phone)
    {
      // Arrange
      var client = factory.CreateClient();
      
      CreateCustomerRequest request = new()
      {
        Title = title,
        Forename = forename,
        Surname = surname,
        Email = email,
        Phone = phone,
      };

      var body = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8)
      {
        Headers = {
          ContentType = new MediaTypeHeaderValue("application/json"),
        }
      };

      // Act
      var response = await client.PostAsync("/customers", body);

      // Assert
      response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithNullBody_ReturnsBadRequest()
    {
      // Arrange
      var client = factory.CreateClient();

      var body = new StringContent(JsonConvert.SerializeObject(null), Encoding.UTF8)
      {
        Headers = {
          ContentType = new MediaTypeHeaderValue("application/json"),
        }
      };

      // Act
      var response = await client.PostAsync("/customers", body);

      // Assert
      response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReadAll_Always_ReturnsListOfCustomers()
    {
      // Arrange
      var client = factory.CreateClient();

      // Act
      var response = await client.GetAsync("/customers");

      // Assert
      response.StatusCode.Should().Be(HttpStatusCode.OK);
      var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();
      customers.Should().NotBeNull();
    }


    [Fact]
    public async Task Delete_ExistentCustomer_ReturnsNoContent()
    {
      // Arrange
      var client = factory.CreateClient();

      CreateCustomerRequest request = new()
      {
        Title = "Mr.",
        Forename = "Deletable",
        Surname = "Customer",
        Email = "to-delete@example.com",
        Phone = "+5216691220511",
      };

      var body = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8)
      {
        Headers = {
          ContentType = new MediaTypeHeaderValue("application/json"),
        }
      };

      var createResponse = await client.PostAsync("/customers", body);      
      var customer = await createResponse.Content.ReadFromJsonAsync<Customer>();

      // Act
      var response = await client.DeleteAsync($"/customers/{customer?.Id}");

      // Assert
      response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
  }
}

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
using System.Text;
using System.Threading.Tasks;

namespace ConsumerManager.Integration.Tests.Controllers
{
  public class CustomersControllerTest : IClassFixture<WebApplicationFactory<Program>>
  {
    private readonly WebApplicationFactory<Program> factory;

    public CustomersControllerTest(WebApplicationFactory<Program> factory)
    {
      this.factory = factory;
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
  }
}

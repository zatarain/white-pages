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
  public class AddressesControllerTest : IClassFixture<TestApplicationFactory<Program>>
  {
    private readonly TestApplicationFactory<Program> factory;

    public AddressesControllerTest(TestApplicationFactory<Program> factory)
    {
      this.factory = factory;
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAddress()
    {
      // Arrange
      var client = factory.CreateClient();

      CreateCustomerRequest customerRequest = new()
      {
        Title = "Mr.",
        Forename = "New",
        Surname = "Customer",
        Email = "mr.new@customer.com",
        Phone = "+4407333222444",
      };

      var customerJSON = new StringContent(JsonConvert.SerializeObject(customerRequest), Encoding.UTF8)
      {
        Headers = {
          ContentType = new MediaTypeHeaderValue("application/json"),
        }
      };

      var customerResponse = await client.PostAsync("/customers", customerJSON);
      var customer = await customerResponse.Content.ReadFromJsonAsync<Customer>();

      CreateAddressRequest addressRequest = new()
      {
        Line1 = "Flat 58",
        Line2 = "416 Manchester Road",
        Town = "London",
        County = "",
        Postcode = "E14 9LR",
        Country = "GB",
      };

      var addressJSON = new StringContent(JsonConvert.SerializeObject(addressRequest), Encoding.UTF8)
      {
        Headers = {
          ContentType = new MediaTypeHeaderValue("application/json"),
        }
      };

      // Act
      var response = await client.PostAsync($"/addresses/{customer?.Id}", addressJSON);

      // Assert
      response.StatusCode.Should().Be(HttpStatusCode.Created);
      var address = await response.Content.ReadFromJsonAsync<Address>();
      addressRequest.Should().BeEquivalentTo(
        address,
        options => options.ComparingByMembers<Address>().ExcludingMissingMembers()
      );
    }
  }
}

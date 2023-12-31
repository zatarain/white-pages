﻿using Castle.Core.Resource;
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
    private readonly Random random = new();

    public AddressesControllerTest(TestApplicationFactory<Program> factory)
    {
      this.factory = factory;
    }

    private async Task<Customer?> CreateRandomCustomer(HttpClient client)
    {
      string code = random.Next(1, 99).ToString();
      string number = random.Next(10000000, 99999999).ToString();
      CreateCustomerRequest request = new()
      {
        Title = "Mr.",
        Forename = "Random",
        Surname = "Customer",
        Email = $"random-{number}@customer.com",
        Phone = $"+{code}111{number}",
      };

      var body = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8)
      {
        Headers = {
          ContentType = new MediaTypeHeaderValue("application/json"),
        }
      };

      var createResponse = await client.PostAsync("/customers", body);
      var created = await createResponse.Content.ReadFromJsonAsync<Customer>();
      return created;
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAddress()
    {
      // Arrange
      var client = factory.CreateClient();
      var customer = await CreateRandomCustomer(client) ;

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

    private async Task<Address?> CreateRandomAddress(HttpClient client, int customerId)
    {
      var flat = random.Next(10, 99).ToString();
      var streetNumber = random.Next(100, 500).ToString();
      CreateAddressRequest addressRequest = new()
      {
        Line1 = $"Flat {flat}",
        Line2 = $"{streetNumber} Manchester Road",
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

      var response = await client.PostAsync($"/addresses/{customerId}", addressJSON);

      var address = await response.Content.ReadFromJsonAsync<Address>();
      return address;
    }

    [Theory]
    [InlineData(null, "Liverpool", "L4 7AB", "GB")]
    [InlineData("Flat 12", null, "L4 7AB", "GB")]
    [InlineData("Flat 13", "Liverpool", null, "GB")]
    [InlineData("Flat 14", "Liverpool", "L4 7AB", "XB")]
    [InlineData("Flat 15", "Liverpool", "L4 777", "GB")]
    [InlineData("Flat 21", "Dallas", "90AC7", "US")]
    [InlineData("Flat 22", "Mexico City", "L4 7AB", "MX")]
    public async Task Create_WithInvalidData_ReturnsBadRequest(string line1, string town, string postcode, string country)
    {
      // Arrange
      var client = factory.CreateClient();
      var customer = await CreateRandomCustomer(client);

      CreateAddressRequest addressRequest = new()
      {
        Line1 = line1,
        Line2 = "416 Manchester Road",
        Town = town,
        County = "",
        Postcode = postcode,
        Country = country,
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
      response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_FromCustomerWithMultipleAddress_ReturnsNoContent()
    {
      // Arrange
      var client = factory.CreateClient();
      var customer = await CreateRandomCustomer(client);
      var address1 = await CreateRandomAddress(client, customer.Id);
      var address2 = await CreateRandomAddress(client, customer.Id);

      // Act
      var response = await client.DeleteAsync($"/addresses/{address1?.Id}");

      // Assert
      response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
  }
}

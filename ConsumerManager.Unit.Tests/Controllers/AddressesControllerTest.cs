using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ConsumerManager.Entities;
using ConsumerManager.Controllers;
using Microsoft.Extensions.Logging;
using ConsumerManager.Entities.Database;
using Microsoft.EntityFrameworkCore;
using Castle.Core.Resource;
using System.Reflection.Metadata;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;

namespace ConsumerManager.Unit.Tests.Controllers
{
  public class AddressesControllerTest
  {
    private readonly Mock<ILogger<CustomersController>> loggerMock = new();
    private readonly Mock<AsyncDataService> serviceMock;

    public AddressesControllerTest()
    {
      var modelMock = new Mock<RelationalModel>();
      serviceMock = new(modelMock.Object);
    }

    private static Address CreateTestAddress()
    {
      var now = DateTime.UtcNow;
      return new()
      {
        Id = 4,
        CustomerId = 1,
        Line1 = "Flat 101",
        Line2 = "Gainsborough House, Cassilis Road",
        Town = "London",
        County = "-",
        Postcode = "E14 9LR",
        Country = "GB",
        CreatedAt = now,
        LastUpdatedAt = now,
      };
    }

    private static Customer CreateTestCustomer()
    {
      var now = DateTime.UtcNow;
      return new()
      {
        Id = 1,
        Title = "Mr.",
        Forename = "John",
        Surname = "Smith",
        Email = "john.smith@example.com",
        Phone = "+4407111222333",
        IsActive = true,
        MainAddressId = 4,
        Addresses = new List<Address>(),
        CreatedAt = now,
        LastUpdatedAt = now,
      };
    }

    [Fact]
    public async Task Read_ExistentCustomerByAddressId_ReturnsCustomerWithAddress()
    {
      // Arrange
      var address = CreateTestAddress();
      var customer = CreateTestCustomer() with {
        Addresses = new List<Address>() { address }
      };

      serviceMock.Setup(mock => mock.GetCustomerByAddressId(It.IsAny<int>())).ReturnsAsync(customer);
      var controller = new AddressesController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Read(1);

      // Assert
      actual.Result.Should().BeOfType<OkObjectResult>();
      actual.Value?.Addresses.Should().Contain(address);
    }

    [Fact]
    public async Task Read_UnexistentCustomerByAddressId_ReturnsNotFound()
    {
      // Arrange
      serviceMock.Setup(mock => mock.GetCustomerByAddressId(It.IsAny<int>())).ReturnsAsync(null as Customer);
      var controller = new AddressesController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Read(1);

      // Assert
      actual.Result.Should().BeOfType<NotFoundResult>();
    }
  }
}

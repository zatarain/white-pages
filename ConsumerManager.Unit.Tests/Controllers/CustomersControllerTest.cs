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

namespace ConsumerManager.Unit.Tests.Controllers
{
  public class CustomersControllerTest
  {
    private readonly Mock<ILogger<CustomersController>> loggerMock = new();
    private readonly Mock<AsyncDataService> serviceMock;

    public CustomersControllerTest()
    {
      var modelMock = new Mock<RelationalModel>();
      serviceMock = new(modelMock.Object);
    }

    private static Customer CreateTestCustomer()
    {
      return new()
      {
        Id = 1,
        Title = "Mr.",
        Forename = "John",
        Surname = "Smith",
        Email = "john.smith@gmail.com",
        Phone = "+4407111222333",
        IsActive = true,
        Addresses = new List<Address>(),
        CreatedAt = DateTime.Now,
        LastUpdatedAt = DateTime.Now,
      };
    }

    [Fact]
    public async Task ReadAll_Always_ReturnsListOfCustomers()
    {
      // Arrange
      var data = new List<Customer>
      {
        CreateTestCustomer(),
      };
      
      serviceMock.Setup(mock => mock.GetAllCustomers()).ReturnsAsync(data);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.ReadAll();

      // Assert
      actual.Result.Should().BeOfType<OkObjectResult>();
      var result = actual.Result as OkObjectResult;
      result?.Value.Should().BeEquivalentTo(data.ToList());
    }

    [Fact]
    public async Task Read_UnexistentCustomer_ReturnsNotFound()
    {
      // Arrange
      serviceMock.Setup(mock => mock.GetCustomerById(It.IsAny<int>())).ReturnsAsync(null as Customer);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Read(2);

      // Assert
      actual.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Read_ExistentCustomer_ReturnsCustomer()
    {
      // Arrange
      var customer = CreateTestCustomer();
      serviceMock.Setup(mock => mock.GetCustomerById(It.IsAny<int>())).ReturnsAsync(customer);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Read(1);

      // Assert
      actual.Result.Should().BeOfType<OkObjectResult>();
      var result = actual.Result as OkObjectResult;
      result?.Value.Should().BeEquivalentTo(customer, options => options.ComparingByMembers<Customer>());
    }

    [Fact]
    public async Task Create_WithNullInputData_ReturnsBadrequest()
    {
      // Arrange
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Create(null);

      // Assert
      actual?.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Delete_ExistentCustomer_ReturnsSuccessWithNoContent()
    {
      var customer = CreateTestCustomer();

      // Arrange
      serviceMock.Setup(mock => mock.GetCustomerById(It.IsAny<int>())).ReturnsAsync(customer);
      serviceMock.Setup(mock => mock.DeleteCustomer(It.IsAny<Customer>())).Returns(Task.CompletedTask);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Delete(1);

      // Assert
      actual?.Result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_UnexistentCustomer_ReturnsNotFound()
    {
      // Arrange
      serviceMock.Setup(mock => mock.GetCustomerById(It.IsAny<int>())).ReturnsAsync(null as Customer);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Delete(4);

      // Assert
      actual?.Result.Should().BeOfType<NotFoundResult>();
    }
  }
}

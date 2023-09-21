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
        Email = "john.smith@example.com",
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
    public async Task Create_WithValidData_ReturnsCreatedCustomer()
    {
      // Arrange
      serviceMock.Setup(mock => mock.CustomerExists(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
      serviceMock.Setup(mock => mock.CreateCustomer(It.IsAny<Customer>())).Returns(Task.CompletedTask);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);
      
      CreateCustomerContract request = new()
      {
        Title = "Mr.",
        Forename = "New",
        Surname = "Customer",
        Email = "john.smith@example.com",
        Phone = "+4407111222333",
      };

      // Act
      var actual = await controller.Create(request);

      // Assert
      actual?.Result.Should().BeOfType<CreatedAtActionResult>();
      var result = actual?.Result as CreatedAtActionResult;
      var createdCustomer = result?.Value as Customer;
      request.Should().BeEquivalentTo(
        createdCustomer,
        options => options.ComparingByMembers<Customer>().ExcludingMissingMembers()
      );
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
    public async Task Create_WithExistentData_ReturnsBadrequest()
    {
      // Arrange
      serviceMock.Setup(mock => mock.CustomerExists(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);
      CreateCustomerContract request = new() {
        Title = "Mr.",
        Forename = "New",
        Surname = "Customer",
        Email = "john.smith@example.com",
        Phone = "+4407111222333",
      };

      // Act
      var actual = await controller.Create(request);

      // Assert
      actual?.Result.Should().BeOfType<BadRequestObjectResult>();
      actual?.Value?.Should().Be($"A customer with the email '{request.Email}' and/or phone '{request.Phone}' already exists.");
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
      actual.Should().BeOfType<NoContentResult>();
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
      actual.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Activate_ExistentCustomer_ReturnsActivatedCustomer()
    {
      // Arrange
      var customer = CreateTestCustomer();
      customer.IsActive = false;
      serviceMock.Setup(mock => mock.UpdateCustomerStatus(It.IsAny<int>(), true)).ReturnsAsync(customer);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Activate(1);

      // Assert
      actual?.Result.Should().BeOfType<OkObjectResult>();
      actual?.Value?.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Activate_UnexistentCustomer_ReturnsNotFound()
    {
      // Arrange
      serviceMock.Setup(mock => mock.UpdateCustomerStatus(It.IsAny<int>(), true)).ReturnsAsync(null as Customer);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Activate(4);

      // Assert
      actual?.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Dectivate_ExistentCustomer_ReturnsActivatedCustomer()
    {
      // Arrange
      var customer = CreateTestCustomer();
      customer.IsActive = true;
      serviceMock.Setup(mock => mock.UpdateCustomerStatus(It.IsAny<int>(), false)).ReturnsAsync(customer);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Deactivate(1);

      // Assert
      actual?.Result.Should().BeOfType<OkObjectResult>();
      actual?.Value?.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Deactivate_UnexistentCustomer_ReturnsNotFound()
    {
      // Arrange
      serviceMock.Setup(mock => mock.UpdateCustomerStatus(It.IsAny<int>(), false)).ReturnsAsync(null as Customer);
      var controller = new CustomersController(loggerMock.Object, serviceMock.Object);

      // Act
      var actual = await controller.Deactivate(4);

      // Assert
      actual?.Result.Should().BeOfType<NotFoundResult>();
    }
  }
}

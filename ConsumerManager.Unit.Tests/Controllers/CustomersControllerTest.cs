﻿using System;
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
    [Fact]
    public async Task ReadAll_Always_ReturnsListOfCustomers()
    {
      // Arrange
      var data = new List<Customer>
      {
        new Customer
        {
          Id = 1,
          Title = "Mr.",
          Forename = "John",
          Surename = "Smith",
          Email = "john.smith@gmail.com",
          Phone = "+4407111111111",
          Addresses = new List<Address>(),
          CreatedAt = DateTime.Now,
          LastUpdatedAt = DateTime.Now,
        }
      }.AsQueryable();
      
      var loggerMock = new Mock<ILogger<CustomersController>>();
      
      var customersMock = new Mock<DbSet<Customer>>();
      customersMock.As<IQueryable<Customer>>().Setup(mock => mock.Provider).Returns(data.Provider);
      customersMock.As<IQueryable<Customer>>().Setup(mock => mock.Expression).Returns(data.Expression);
      customersMock.As<IQueryable<Customer>>().Setup(mock => mock.ElementType).Returns(data.ElementType);
      customersMock.As<IQueryable<Customer>>().Setup(mock => mock.GetEnumerator()).Returns(() => data.GetEnumerator());
      
      var modelMock = new Mock<RelationalModel>();
      modelMock.Setup(mock => mock.Customers).Returns(customersMock.Object);
      
      var controller = new CustomersController(loggerMock.Object, modelMock.Object);

      // Act
      var actual = await controller.ReadAll();

      // Assert
      actual.Should().NotBeNull();
      var customers = actual.Value;
      customers.Should().NotBeNull();
      customers.Should().HaveCount(1);
      customers.Should().BeEquivalentTo(data.ToList());
    }

    [Fact]
    public async Task Read_UnexistentCustomer_ReturnsNotFound()
    {
      // Arrange
      var loggerMock = new Mock<ILogger<CustomersController>>();

      var customersMock = new Mock<DbSet<Customer>>();
      customersMock.Setup(mock => mock.FindAsync(It.IsAny<int>())).ReturnsAsync(null as Customer);

      var modelMock = new Mock<RelationalModel>();
      modelMock.Setup(mock => mock.Customers).Returns(customersMock.Object);

      var controller = new CustomersController(loggerMock.Object, modelMock.Object);

      // Act
      var actual = await controller.Read(2);

      // Assert
      actual.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Read_ExistentCustomer_ReturnsCustomer()
    {
      // Arrange
      var customer = new Customer
      {
        Id = 1,
        Title = "Mr.",
        Forename = "John",
        Surename = "Smith",
        Email = "john.smith@gmail.com",
        Phone = "+4407111222333",
        Addresses = new List<Address>(),
        CreatedAt = DateTime.Now,
        LastUpdatedAt = DateTime.Now,
      };

      var loggerMock = new Mock<ILogger<CustomersController>>();

      var customersMock = new Mock<DbSet<Customer>>();
      customersMock.Setup(mock => mock.FindAsync(It.IsAny<int>())).ReturnsAsync(customer);

      var modelMock = new Mock<RelationalModel>();
      modelMock.Setup(mock => mock.Customers).Returns(customersMock.Object);

      var controller = new CustomersController(loggerMock.Object, modelMock.Object);

      // Act
      var actual = await controller.Read(1);

      // Assert
      actual.Should().NotBeNull();
      actual.Value.Should().BeEquivalentTo(customer, options => options.ComparingByMembers<Customer>());
    }

  [Fact]
  public async Task Create_WithNullInputData_ReturnsBadrequest()
  {
    // Arrange
    var loggerMock = new Mock<ILogger<CustomersController>>();

    var customersMock = new Mock<DbSet<Customer>>();
    // customersMock.Setup(mock => mock.FirstOrDefaultAsync(It.IsAny<Expression<Func<Customer, bool>>>())).ReturnsAsync(null);

    var modelMock = new Mock<RelationalModel>();
    modelMock.Setup(mock => mock.Customers).Returns(customersMock.Object);

    var controller = new CustomersController(loggerMock.Object, modelMock.Object);

    // Act
    var actual = await controller.Create(null);

      // Assert
      actual?.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
}

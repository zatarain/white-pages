namespace ConsumerManager.Unit.Tests;

using Moq;
using ConsumerManager;
using ConsumerManager.Controllers;
using FluentAssertions;
using Microsoft.Extensions.Logging;

public class WeatherForecastControllerTests
{
	[Fact]
	public void Get_Always_ReturnsForecast()
	{
		// Arrange
		var loggerMock = new Mock<ILogger<WeatherForecastController>>();
		var controller = new WeatherForecastController(loggerMock.Object);

		// Act
		var result = controller.Get() as WeatherForecast[];

		// Assert
		result.Should().HaveCount(5);
		loggerMock.Verify(logger => logger.Log(
				It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
				It.Is<EventId>(eventId => eventId.Id == 0),
				It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Getting five weather forecasts" && @type.Name == "FormattedLogValues"),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
		Times.Once);
	}
}

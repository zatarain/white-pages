namespace ConsumerManager.Integration.Tests;

using ConsumerManager.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
public class WeatherForecastControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly WebApplicationFactory<Program> factory;

	public WeatherForecastControllerTest(WebApplicationFactory<Program> factory)
	{
		this.factory = factory;
	}

	[Fact]
	public async Task Get_Always_SuccessReturningFiveItems()
	{
		var valp = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
		System.Console.WriteLine($"p-value: {valp}");
    // Arrange
    var client = factory.CreateClient();

		// Act
		var response = await client.GetAsync("/WeatherForecast");

		// Assert
		response.IsSuccessStatusCode.Should().BeTrue();
		var forecasts = await response.Content.ReadFromJsonAsync<List<WeatherForecast>>();
		forecasts.Should().NotBeNull();
		forecasts?.Count.Should().Be(5);
	}
}

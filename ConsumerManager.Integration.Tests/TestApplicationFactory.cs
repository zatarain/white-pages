using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using ConsumerManager.Entities.Database;
using Microsoft.AspNetCore.Hosting;

namespace ConsumerManager.Integration.Tests
{
  public class TestApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.UseEnvironment("Test");
    }
  }
}

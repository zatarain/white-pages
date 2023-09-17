using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConsumerManager.Entities.Database
{
  public class RelationalModel : DbContext
  {
    public DbSet<WeatherForecast> WeatherForecast { get; set; }

    public RelationalModel() : base()
    {

    }

    public RelationalModel(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<WeatherForecast>().HasNoKey();
      //.HasOne(m => m.Publisher).WithMany(m => m.Books);
    }
  }
}

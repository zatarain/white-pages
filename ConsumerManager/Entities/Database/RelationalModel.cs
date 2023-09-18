using ConsumerManager.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ConsumerManager.Entities.Database
{
  public class RelationalModel : DbContext
  {
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Address> Addresses { get; set; }

    public RelationalModel() : base()
    {

    }

    public RelationalModel(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      builder.Entity<Address>().HasOne(model => model.Customer).WithMany(model => model.Addresses);
    }
  }
}

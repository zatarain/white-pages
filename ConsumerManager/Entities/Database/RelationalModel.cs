﻿using ConsumerManager.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ConsumerManager.Entities.Database
{
  public class RelationalModel : DbContext
  {
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Address> Addresses { get; set; }

    public RelationalModel() : base()
    {

    }

    public RelationalModel(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      builder.Entity<Customer>(customer => customer.HasMany(customer => customer.Addresses));
      builder.Entity<Address>(address => address.HasIndex(address => address.CustomerId));
    }
  }
}

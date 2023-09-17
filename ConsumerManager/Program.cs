using ConsumerManager.Entities.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Environment Variables.
builder.Configuration.AddEnvironmentVariables();

// Add database connection and context.
var postgres = builder.Configuration.GetSection("Postgres").Get<PostgresConnection>();
builder.Services.AddDbContext<RelationalModel>(options => options.UseNpgsql(postgres.ToString()));

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }

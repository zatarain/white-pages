using ConsumerManager.Entities.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add Environment Variables.
builder.Configuration.AddEnvironmentVariables();

// Add database connection and context.
var postgres = builder.Configuration.GetSection("Postgres").Get<PostgresConnection>();
builder.Services.AddDbContext<RelationalModel>(options => options.UseNpgsql(postgres?.ToString()));
// Migrate latest database changes during startup
builder.Services.AddScoped<IDataServiceProvider, AsyncDataService>();

// Add services to the container.
builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
  });

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

// Migrate latest database changes during startup
using (var scope = app.Services.CreateScope())
{
  var model = scope.ServiceProvider.GetRequiredService<RelationalModel>();
  model.Database.Migrate();
}

// Don't use HTTPS as this will be handover to the Application Load Balancer
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }

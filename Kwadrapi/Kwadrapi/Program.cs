using Infrastructure.Data;
using Kwadrapi;
using Kwadrapi.Routing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MqttService>();

// Add the domain context to the container
builder.Services.AddDbContext<DomainContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add controllers
builder.Services.AddControllers(options => { options.Conventions.Add(new VersionRouteConvention()); });


var app = builder.Build();

// Apply any pending database migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DomainContext>();
    dbContext.Database.Migrate();
    Console.WriteLine("Applied database migrations.");
}

// Test database connection
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DomainContext>();
    dbContext.Database.OpenConnection();
    Console.WriteLine("Connected to the database successfully.");
    dbContext.Database.CloseConnection();
}

// Connect to MQTT broker
var mqttService = app.Services.GetRequiredService<MqttService>();
await mqttService.ConnectAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

app.Run();
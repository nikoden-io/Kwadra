var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "User MicroService Entry Point");

app.Run();
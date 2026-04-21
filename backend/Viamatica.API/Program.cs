using Microsoft.EntityFrameworkCore;
using Viamatica.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ViamaticaDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    status = "ok",
    service = "Viamatica.API"
}));

app.MapControllers();

app.Run();

public partial class Program;

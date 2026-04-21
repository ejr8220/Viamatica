var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    status = "ok",
    service = "Viamatica.API"
}));

app.MapControllers();

app.Run();

public partial class Program;

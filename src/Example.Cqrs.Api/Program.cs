using Example.Cqrs.Api.Endpoints;
using Example.Cqrs.Application;
using Example.Cqrs.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapProductEndpoints();
app.MapOrderEndpoints();

app.Run();

namespace Example.Cqrs.Api
{
    public partial class Program;
}

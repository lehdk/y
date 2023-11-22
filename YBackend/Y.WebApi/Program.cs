using Microsoft.EntityFrameworkCore;
using Y.Infrastructure;
using Y.Infrastructure.Extensions;
using Y.Application.Extensions;
using Y.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

string? connectionString = builder.Configuration.GetConnectionString("database");

builder.Services.AddDbContext<DatabaseContext>(opt =>
{
    opt.UseSqlServer(connectionString, b => b.MigrationsAssembly("Y.WebApi"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddYInfrastructure();
builder.Services.AddYApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

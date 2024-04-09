using Classifieds.Data;
using Classifieds_API.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(
    dbContextOptions => dbContextOptions.UseSqlServer(connectionString)
);


builder.Services.AddSwagger();
builder.Services.AddJwt(configuration);
builder.Services.AddServices();
builder.Services.AddCloudinary(configuration);

var app = builder.Build();

var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
try
{
    var dataContext = serviceProvider.GetRequiredService<DataContext>();
    dataContext.Database.EnsureCreated();
    Console.WriteLine("MIGRATIONNNNNNNNNNNN");
}
catch
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError("Migration failed");
}
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

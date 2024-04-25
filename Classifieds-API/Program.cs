using Classifieds.Data;
using Classifieds_API.Extensions;
using Classifieds_API.Middleware;
using Common;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

// Configure the application's configuration settings
builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
// Map AppSettings section in appsettings.json file value to AppSetting model
builder.Configuration.GetSection("AppSettings").Get<AppSettings>(options => options.BindNonPublicProperties = true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(
    dbContextOptions => dbContextOptions.UseSqlServer(AppSettings.ConnectionStrings)
);


builder.Services.AddSwagger();
builder.Services.AddJwt(configuration);
builder.Services.AddServices();
builder.Services.AddCloudinary(configuration);
builder.Services.AddAutoMapper();
builder.Services.AddCORS();

var app = builder.Build();

var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
try
{
    var dataContext = serviceProvider.GetRequiredService<DataContext>();
    dataContext.Database.EnsureCreated();
    Console.WriteLine("MIGRATIONNNNNNNNNNNN");
    Console.WriteLine("Seed category");
    DbInitialize.Initialize(dataContext);
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

app.UseCors("AllowAll");
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<JwtMiddleware>();
app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();

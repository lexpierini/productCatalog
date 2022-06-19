using backendMinimalApi.AppServicesExtensions;
using backendMinimalApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. = ConfigureServices (startup)
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.AddApiSwagger();

// PostgreSQL connection settings
builder.AddPersistence();

builder.Services.AddCors();

// Security Token
builder.AddAuthenticationJwt();

var app = builder.Build();

#region EndPoints
app.MapAuthenticationEP();
app.MapCategoriesEP();
app.MapProductsEP();

#endregion

// Configure the HTTP request pipeline. == Configure (startup)
var environment = app.Environment;

app.UseExceptionHandling(environment)
   .UseSwaggerMiddleware()
   .UseAppCors();

// Activation of authentication and authorization services
app.UseAuthentication();
app.UseAuthorization();

app.Run();

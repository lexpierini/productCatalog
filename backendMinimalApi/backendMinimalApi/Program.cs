using backendMinimalApi.Context;
using backendMinimalApi.Models;
using backendMinimalApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. = ConfigureServices (startup)
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiProductCatalog", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme.
                    Enter 'Bearer'[space].Example: \'Bearer 12345abcdef\'",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
});

// PostgreSQL connection settings -------------------------------------------------------- //
string postgreSqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(postgreSqlConnection));
// --------------------------------------------------------------------------------------- //


// Security Token ------------------------------------------------------------------------ //
builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthentication
                 (JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,

                         ValidIssuer = builder.Configuration["Jwt:Issuer"],
                         ValidAudience = builder.Configuration["Jwt:Audience"],
                         IssuerSigningKey = new SymmetricSecurityKey
                         (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                     };
                 });

builder.Services.AddAuthorization();
// --------------------------------------------------------------------------------------- //


var app = builder.Build();


// EndPoints ----------------------------------------------------------------------------- //
// Login
app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
{
    if (userModel == null) return Results.BadRequest("Invalid Login");

    if (userModel.UserName == "admin" && userModel.Password == "admin#123")
    {
        var tokenString = tokenService.MakeToken(app.Configuration["Jwt:Key"],
            app.Configuration["Jwt:Issuer"],
            app.Configuration["Jwt:Audience"],
            userModel);

        return Results.Ok(new { token = tokenString });
    }
    else
    {
        return Results.BadRequest("Invalid Login");
    }
}).Produces(StatusCodes.Status400BadRequest)
  .Produces(StatusCodes.Status200OK)
  .WithName("Login")
  .WithTags("Authentication");


// Category
app.MapGet("/GetAllCategories", async (AppDbContext db) => await db.Categories.ToListAsync()).WithTags("Categories").RequireAuthorization();

app.MapGet("/GetOneCategory/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Categories.FindAsync(id) 
                 is Category category
                 ? Results.Ok(category)
                 : Results.NotFound();
}).WithTags("Categories").RequireAuthorization();

app.MapPost("/AddOneCategory", async (Category category, AppDbContext db) =>
{
    db.Categories.Add(category);
    await db.SaveChangesAsync();

    return Results.Created($"/GetOneCategory/{category.CategoryId}", category);
}).WithTags("Categories").RequireAuthorization();

app.MapPut("/UpdateOneCategory/{id:int}", async (int id, Category category, AppDbContext db) =>
{
    if (category.CategoryId != id) return Results.BadRequest();

    var categoryDB = await db.Categories.FindAsync(id);

    if (categoryDB is null) return Results.NotFound();

    categoryDB.Name = category.Name;

    await db.SaveChangesAsync();

    return Results.Ok(categoryDB);
}).WithTags("Categories").RequireAuthorization();

app.MapDelete("/DeleteOneCategory/{id:int}", async (int id, AppDbContext db) =>
{
    var category = await db.Categories.FindAsync(id);

    if (category is null) return Results.NotFound();

    db.Categories.Remove(category);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).WithTags("Categories").RequireAuthorization();


// Product
app.MapGet("/GetAllProducts", async (AppDbContext db) => await db.Products.ToListAsync()).WithTags("Products").RequireAuthorization();

app.MapGet("/GetOneProduct/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Products.FindAsync(id)
                 is Product product
                 ? Results.Ok(product)
                 : Results.NotFound();
}).WithTags("Products").RequireAuthorization();

app.MapPost("/AddOneProduct", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/GetOneProduct/{product.ProductId}", product);
}).WithTags("Products").RequireAuthorization();

app.MapPut("/UpdateOneProduct/{id:int}", async (int id, Product product, AppDbContext db) =>
{
    if (product.ProductId != id) return Results.BadRequest();

    var productDB = await db.Products.FindAsync(id);

    if (productDB is null) return Results.NotFound();

    productDB.Name = product.Name;
    productDB.Description = product.Description;
    productDB.Price = product.Price;
    productDB.ImageUrl = product.ImageUrl;
    productDB.Inventory = product.Inventory;
    productDB.RegistrationDate = product.RegistrationDate;
    productDB.CategoryId = product.CategoryId;

    await db.SaveChangesAsync();

    return Results.Ok(productDB);
}).WithTags("Products").RequireAuthorization();

app.MapDelete("/DeleteOneProduct/{id:int}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product.ProductId != id) return Results.BadRequest();

    db.Products.Remove(product);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).WithTags("Products").RequireAuthorization();
// --------------------------------------------------------------------------------------- //


// Configure the HTTP request pipeline. == Configure (startup)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activation of authentication and authorization services
app.UseAuthentication();
app.UseAuthorization();


app.Run();

using backendMinimalApi.Context;
using backendMinimalApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. = ConfigureServices (startup)
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PostgreSQL connection settings -------------------------------------------------------- //
string postgreSqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(postgreSqlConnection));
// --------------------------------------------------------------------------------------- //

var app = builder.Build();


// EndPoints ----------------------------------------------------------------------------- //
// Category
app.MapGet("/GetAllCategories", async (AppDbContext db) => await db.Categories.ToListAsync());

app.MapGet("/GetOneCategory/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Categories.FindAsync(id) 
                 is Category category
                 ? Results.Ok(category)
                 : Results.NotFound();
});

app.MapPost("/AddOneCategory", async (Category category, AppDbContext db) =>
{
    db.Categories.Add(category);
    await db.SaveChangesAsync();

    return Results.Created($"/GetOneCategory/{category.CategoryId}", category);
});

app.MapPut("/UpdateOneCategory/{id:int}", async (int id, Category category, AppDbContext db) =>
{
    if (category.CategoryId != id) return Results.BadRequest();

    var categoryDB = await db.Categories.FindAsync(id);

    if (categoryDB is null) return Results.NotFound();

    categoryDB.Name = category.Name;

    await db.SaveChangesAsync();

    return Results.Ok(categoryDB);
});

app.MapDelete("/DeleteOneCategory/{id:int}", async (int id, AppDbContext db) =>
{
    var category = await db.Categories.FindAsync(id);

    if (category is null) return Results.NotFound();

    db.Categories.Remove(category);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Product
app.MapGet("/GetAllProducts", async (AppDbContext db) => await db.Products.ToListAsync());

app.MapGet("/GetOneProduct/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Products.FindAsync(id)
                 is Product product
                 ? Results.Ok(product)
                 : Results.NotFound();
});

app.MapPost("/AddOneProduct", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/GetOneProduct/{product.ProductId}", product);
});

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
});

app.MapDelete("/DeleteOneProduct/{id:int}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product.ProductId != id) return Results.BadRequest();

    db.Products.Remove(product);
    await db.SaveChangesAsync();

    return Results.NoContent();
});
// --------------------------------------------------------------------------------------- //


// Configure the HTTP request pipeline. = Configure (startup)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

using backendMinimalApi.Context;
using backendMinimalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace backendMinimalApi.Endpoints
{
    public static class Products
    {
        public static void MapProductsEP(this WebApplication app)
        {
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
        }
    }
}

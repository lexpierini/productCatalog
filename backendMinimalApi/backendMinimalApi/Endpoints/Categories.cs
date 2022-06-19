using backendMinimalApi.Context;
using backendMinimalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace backendMinimalApi.Endpoints
{
    public static class Categories
    {
        public static void MapCategoriesEP(this WebApplication app)
        {
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
        }
    }
}

using backendMinimalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace backendMinimalApi.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product>? Products { get; set; }
        public DbSet<Category>? Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Category>().HasKey(c => c.CategoryId);
            mb.Entity<Category>().Property(c => c.Name).HasMaxLength(100).IsRequired();

            mb.Entity<Product>().HasKey(p => p.ProductId);
            mb.Entity<Product>().Property(p => p.Name).HasMaxLength(100).IsRequired();
            mb.Entity<Product>().Property(p => p.Description).HasMaxLength(150);
            mb.Entity<Product>().Property(p => p.ImageUrl).HasMaxLength(100);
            mb.Entity<Product>().Property(p => p.Price).HasPrecision(14, 2);

            // Relationship
            mb.Entity<Product>().HasOne<Category>(c => c.Category).WithMany(p => p.Products).HasForeignKey(c => c.CategoryId);
        }
    }
}

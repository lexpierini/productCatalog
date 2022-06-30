using Microsoft.EntityFrameworkCore;
using productCatalog.Context;
using productCatalog.Models;

namespace productCatalog.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public IEnumerable<Category> GetCategoriesProducts()
        {
            return GetAll().Include(c => c.Products);
        }
    }
}

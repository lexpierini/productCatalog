using Microsoft.EntityFrameworkCore;
using productCatalog.Context;
using productCatalog.Models;
using productCatalog.Pagination;

namespace productCatalog.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<PagedList<Category>> GetCategories(CategoriesParameters categoriesParameters)
        {
            return await PagedList<Category>.ToPagedList(
                GetAll().OrderBy(c => c.Name),
                categoriesParameters.PageNumber,
                categoriesParameters.PageSize
            );
        }

        public async Task<IEnumerable<Category>> GetCategoriesProducts()
        {
            return await GetAll().Include(c => c.Products).ToListAsync();
        }
    }
}

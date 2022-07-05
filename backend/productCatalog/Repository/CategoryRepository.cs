using Microsoft.EntityFrameworkCore;
using productCatalog.Context;
using productCatalog.Models;
using productCatalog.Pagination;

namespace productCatalog.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public PagedList<Category> GetCategories(CategoriesParameters categoriesParameters)
        {
            return PagedList<Category>.ToPagedList(
                GetAll().OrderBy(c => c.Name),
                categoriesParameters.PageNumber,
                categoriesParameters.PageSize
            );
        }

        public IEnumerable<Category> GetCategoriesProducts()
        {
            return GetAll().Include(c => c.Products);
        }
    }
}

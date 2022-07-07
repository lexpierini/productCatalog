using productCatalog.Models;
using productCatalog.Pagination;

namespace productCatalog.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<PagedList<Category>> GetCategories(CategoriesParameters categoriesParameters);
        Task<IEnumerable<Category>> GetCategoriesProducts();
    }
}

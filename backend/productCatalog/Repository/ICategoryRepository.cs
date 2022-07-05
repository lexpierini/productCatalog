using productCatalog.Models;
using productCatalog.Pagination;

namespace productCatalog.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        PagedList<Category> GetCategories(CategoriesParameters categoriesParameters);
        IEnumerable<Category> GetCategoriesProducts();
    }
}

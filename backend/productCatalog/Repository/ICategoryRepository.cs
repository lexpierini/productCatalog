using productCatalog.Models;

namespace productCatalog.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        IEnumerable<Category> GetCategoriesProducts();
    }
}

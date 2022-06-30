using productCatalog.Models;

namespace productCatalog.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetProductByPrice();
    }
}

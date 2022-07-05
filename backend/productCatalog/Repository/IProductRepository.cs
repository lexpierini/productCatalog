using productCatalog.Models;
using productCatalog.Pagination;

namespace productCatalog.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        PagedList<Product> GetProducts(ProductsParameters productsParameters);
        IEnumerable<Product> GetProductByPrice();
    }
}

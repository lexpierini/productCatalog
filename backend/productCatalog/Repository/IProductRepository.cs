using productCatalog.Models;
using productCatalog.Pagination;

namespace productCatalog.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<PagedList<Product>> GetProducts(ProductsParameters productsParameters);
        Task<IEnumerable<Product>> GetProductByPrice();
    }
}

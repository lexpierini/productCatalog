using productCatalog.Context;
using productCatalog.Models;
using productCatalog.Pagination;

namespace productCatalog.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public PagedList<Product> GetProducts(ProductsParameters productsParameters)
        {
            return PagedList<Product>.ToPagedList(
                GetAll().OrderBy(p => p.Name),
                productsParameters.PageNumber,
                productsParameters.PageSize
            );
        }

        public IEnumerable<Product> GetProductByPrice()
        {
            return GetAll().OrderBy(p => p.Price).ToList();
        }
    }
}

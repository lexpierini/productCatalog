using productCatalog.Context;
using productCatalog.Models;

namespace productCatalog.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public IEnumerable<Product> GetProductByPrice()
        {
            return GetAll().OrderBy(p => p.Price).ToList();
        }
    }
}

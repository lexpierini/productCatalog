using Microsoft.EntityFrameworkCore;
using productCatalog.Context;
using productCatalog.Models;
using productCatalog.Pagination;

namespace productCatalog.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<PagedList<Product>> GetProducts(ProductsParameters productsParameters)
        {
            return await PagedList<Product>.ToPagedList(
                GetAll().OrderBy(p => p.Name),
                productsParameters.PageNumber,
                productsParameters.PageSize
            );
        }

        public async Task<IEnumerable<Product>> GetProductByPrice()
        {
            return await GetAll().OrderBy(p => p.Price).ToListAsync();
        }
    }
}

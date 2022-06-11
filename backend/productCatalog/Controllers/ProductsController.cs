using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using productCatalog.Context;
using productCatalog.Models;

namespace productCatalog.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAll()
        {
            try
            {
                var products = _context.Products.AsNoTracking().ToList();
                if (products is null)
                {
                    return NotFound("Products not found");
                }
                return products;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpGet("{id:int}", Name = "GetOneProduct")]
        public ActionResult<Product> GetOne(int id)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
                if (product is null)
                {
                    return NotFound("Products not found");
                }
                return product;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpPost]
        public ActionResult AddOne(Product product)
        {
            try
            {
                if (product is null)
                {
                    return BadRequest("The data entered is invalid");
                }

                _context.Products.Add(product);
                _context.SaveChanges();

                return new CreatedAtRouteResult("GetOneProduct", new { id = product.ProductId }, product);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Update(int id, Product product)
        {
            try
            {
                if (id != product.ProductId)
                {
                    return BadRequest("The id informed does not match the id of the product informed");
                }

                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(product);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteOne(int id)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

                if (product is null)
                {
                    return NotFound("Product not found");
                }

                _context.Products.Remove(product);
                _context.SaveChanges();

                return Ok(product);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }
    }
}

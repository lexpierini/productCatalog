using Microsoft.AspNetCore.Mvc;
using productCatalog.Filter;
using productCatalog.Models;
using productCatalog.Repository;

namespace productCatalog.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uof;

        public ProductsController(IUnitOfWork context)
        {
            _uof = context;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Product>> GetAll()
        {
            try
            {
                var products = _uof.ProductRepository.GetAll().ToList();
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

                var product = _uof.ProductRepository.GetById(p => p.ProductId == id);
                if (product is null)
                {
                    throw new Exception("Error when returning the product by id"); //Midleware exceptions
                    //return NotFound("Products not found");
                }
                return product;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProductByPrice()
        {
            return _uof.ProductRepository.GetProductByPrice().ToList();
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

                _uof.ProductRepository.Add(product);
                _uof.Commit();

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

                _uof.ProductRepository.Update(product);
                _uof.Commit();

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
                var product = _uof.ProductRepository.GetById(p => p.ProductId == id);

                if (product is null)
                {
                    return NotFound("Product not found");
                }

                _uof.ProductRepository.Delete(product);
                _uof.Commit();

                return Ok(product);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using productCatalog.DTOs;
using productCatalog.Filter;
using productCatalog.Models;
using productCatalog.Pagination;
using productCatalog.Repository;
using System.Text.Json;

namespace productCatalog.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork context, IMapper mapper)
        {
            _uof = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll([FromQuery] ProductsParameters productsParameters)
        {
            try
            {
                var products = await _uof.ProductRepository.GetProducts(productsParameters);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.TotalPages,
                    products.HasNext,
                    products.HasPrevious,
                };

                if (products is null)
                {
                    return NotFound("Products not found");
                }

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

                var productsDto = _mapper.Map<List<ProductDTO>>(products);
                return productsDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpGet("{id:int}", Name = "GetOneProduct")]
        public async Task<ActionResult<ProductDTO>> GetOne(int id)
        {
            try
            {

                var product = await _uof.ProductRepository.GetById(p => p.ProductId == id);
                if (product is null)
                {
                    throw new Exception("Error when returning the product by id"); //Midleware exceptions
                    //return NotFound("Products not found");
                }
                var productDto = _mapper.Map<ProductDTO>(product);
                return productDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductByPrice()
        {
            var products = await _uof.ProductRepository.GetProductByPrice();
            var productsDto = _mapper.Map<List<ProductDTO>>(products);
            return Ok(productsDto);
        }

        [HttpPost]
        public async Task<ActionResult> AddOne(ProductDTO productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);

                if (product is null)
                {
                    return BadRequest("The data entered is invalid");
                }

                _uof.ProductRepository.Add(product);
                await _uof.Commit();

                var productDTO = _mapper.Map<ProductDTO>(product);

                return new CreatedAtRouteResult("GetOneProduct", new { id = product.ProductId }, productDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, ProductDTO productDto)
        {
            try
            {
                if (id != productDto.ProductId)
                {
                    return BadRequest("The id informed does not match the id of the product informed");
                }

                var product = _mapper.Map<Product>(productDto);

                _uof.ProductRepository.Update(product);
                await _uof.Commit();

                return Ok(productDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProductDTO>> DeleteOne(int id)
        {
            try
            {
                var product = await _uof.ProductRepository.GetById(p => p.ProductId == id);

                if (product is null)
                {
                    return NotFound("Product not found");
                }

                _uof.ProductRepository.Delete(product);
                await _uof.Commit();

                var productDto = _mapper.Map<ProductDTO>(product);

                return Ok(productDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }
    }
}

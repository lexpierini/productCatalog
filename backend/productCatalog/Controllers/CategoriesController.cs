using Microsoft.AspNetCore.Mvc;
using productCatalog.Models;
using productCatalog.Repository;

namespace productCatalog.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;

        public CategoriesController(IUnitOfWork context, ILogger<CategoriesController> logger)
        {
            _uof = context;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetAll()
        {
            _logger.LogInformation("----- GetAll Categories -----");

            try
            {
                return _uof.CategoryRepository.GetAll().ToList();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpGet("{id:int}", Name = "GetOneCategory")]
        public ActionResult<Category> GetOne(int id)
        {
            try
            {
                var category = _uof.CategoryRepository.GetById(c => c.CategoryId == id);

                if (category is null)
                {
                    return NotFound("Category not found");
                }
                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }

        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategoriesProducts()
        {
            try
            {
                return _uof.CategoryRepository.GetCategoriesProducts().ToList();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpPost]
        public ActionResult AddOne(Category category)
        {
            try
            {
                if (category is null)
                {
                    return BadRequest("The data entered is invalid");
                }

                _uof.CategoryRepository.Add(category);
                _uof.Commit();

                return new CreatedAtRouteResult("GetOneCategory", new { id = category.CategoryId }, category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Update(int id, Category category)
        {
            try
            {
                if (id != category.CategoryId)
                {
                    return BadRequest("The id informed does not match the id of the category informed");
                }

                _uof.CategoryRepository.Update(category);
                _uof.Commit();

                return Ok(category);
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
                var category = _uof.CategoryRepository.GetById(c => c.CategoryId == id);

                if (category is null)
                {
                    return NotFound("Category not found");
                }

                _uof.CategoryRepository.Delete(category);
                _uof.Commit();

                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

    }
}

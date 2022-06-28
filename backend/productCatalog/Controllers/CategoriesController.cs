using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using productCatalog.Context;
using productCatalog.Models;

namespace productCatalog.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            try
            {
                return await _context.Categories.AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpGet("{id:int}", Name = "GetOneCategory")]
        public async Task<ActionResult<Category>> GetOne(int id)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

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
                return _context.Categories.Include(p => p.Products).AsNoTracking().ToList();
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

                _context.Categories.Add(category);
                _context.SaveChanges();

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

                _context.Entry(category).State = EntityState.Modified;
                _context.SaveChanges();

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
                var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);

                if (category is null)
                {
                    return NotFound("Category not found");
                }

                _context.Categories.Remove(category);
                _context.SaveChanges();

                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

    }
}

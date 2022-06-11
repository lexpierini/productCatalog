using Microsoft.AspNetCore.Http;
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
        public ActionResult<IEnumerable<Category>> GetAll()
        {
            return _context.Categories.ToList();
        }

        [HttpGet("{id:int}", Name = "GetOneCategory")]
        public ActionResult<Category> GetOne(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);

            if (category is null)
            {
                return NotFound("Category not found");
            }
            return Ok(category);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategoriesProducts()
        {
            return _context.Categories.Include(p => p.Products).ToList();
        }

        [HttpPost]
        public ActionResult AddOne(Category category)
        {
            if(category is null)
            {
                return BadRequest("The data entered is invalid");
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            return new CreatedAtRouteResult("GetOneCategory", new { id = category.CategoryId }, category);
        }

        [HttpPut("{id:int}")]
        public ActionResult Update(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest("The id informed does not match the id of the category informed");
            }

            _context.Entry(category).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(category);
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteOne(int id)
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

    }
}

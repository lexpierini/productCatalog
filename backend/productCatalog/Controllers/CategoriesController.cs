using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using productCatalog.DTOs;
using productCatalog.Models;
using productCatalog.Pagination;
using productCatalog.Repository;
using System.Text.Json;

namespace productCatalog.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork context, ILogger<CategoriesController> logger, IMapper mapper)
        {
            _uof = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoryDTO>> GetAll([FromQuery] CategoriesParameters categoriesParameters)
        {
            _logger.LogInformation("----- GetAll Categories -----");

            try
            {                
                var categories = _uof.CategoryRepository.GetCategories(categoriesParameters);

                var metadata = new
                {
                    categories.TotalCount,
                    categories.PageSize,
                    categories.CurrentPage,
                    categories.TotalPages,
                    categories.HasNext,
                    categories.HasPrevious,
                };

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

                var categoriesDto = _mapper.Map<List<CategoryDTO>>(categories);
                return categoriesDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpGet("{id:int}", Name = "GetOneCategory")]
        public ActionResult<CategoryDTO> GetOne(int id)
        {
            try
            {
                var category = _uof.CategoryRepository.GetById(c => c.CategoryId == id);

                if (category is null)
                {
                    return NotFound("Category not found");
                }
                var categoryDto = _mapper.Map<CategoryDTO>(category);
                return Ok(categoryDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }

        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoryDTO>> GetCategoriesProducts()
        {
            try
            {
                var categories = _uof.CategoryRepository.GetCategoriesProducts().ToList();
                var categoriesDto = _mapper.Map<List<CategoryDTO>>(categories);
                return Ok(categoriesDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpPost]
        public ActionResult AddOne(CategoryDTO categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                if (category is null)
                {
                    return BadRequest("The data entered is invalid");
                }

                _uof.CategoryRepository.Add(category);
                _uof.Commit();

                var categoryDTO = _mapper.Map<CategoryDTO>(category);

                return new CreatedAtRouteResult("GetOneCategory", new { id = category.CategoryId }, categoryDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Update(int id, CategoryDTO categoryDto)
        {
            try
            {
                if (id != categoryDto.CategoryId)
                {
                    return BadRequest("The id informed does not match the id of the category informed");
                }

                var category = _mapper.Map<Category>(categoryDto);

                _uof.CategoryRepository.Update(category);
                _uof.Commit();

                return Ok(categoryDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoryDTO> DeleteOne(int id)
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

                var categoryDto = _mapper.Map<CategoryDTO>(category);

                return Ok(categoryDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem handling your request");
            }
        }

    }
}

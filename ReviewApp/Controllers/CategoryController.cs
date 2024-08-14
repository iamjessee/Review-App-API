using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewApp.Dto;
using ReviewApp.Interfaces;
using ReviewApp.Models;
using ReviewApp.Repository;

namespace ReviewApp.Controllers
{
    // define route and API controller attributes
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository; // repo for category data access
        private readonly IMapper _mapper; // automapper for DTO conversion

        // constructor for injecting dependencies
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        // get all categories
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories()); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(categories); // return categories
        }

        // get category by id
        [HttpGet("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound(); // return not found if category does not exist
            }

            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(category); // return category
        }

        // get pokemons by category id
        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Pokemon>))]

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPokemonByCategoryId(int categoryId)
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonBycategory(categoryId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(pokemons); // return pokemons

        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            try
            {
                if (categoryCreate == null)
                {
                    return BadRequest(ModelState); // return bad request if input is null
                }

                // check if the category already exists
                var category = _categoryRepository.GetCategories()
                    .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (category != null)
                {
                    ModelState.AddModelError("", "Category already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState); // return conflict if category exists
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // return bad request if model state is invalid
                }

                var categoryMap = _mapper.Map<Category>(categoryCreate); // map dto to model

                if (!_categoryRepository.CreateCategory(categoryMap))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState); // return server error if save fails
                }

                return Ok("Successfully created"); // return success message
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong while creating the category"); // return server error
            }

        }
    }
}
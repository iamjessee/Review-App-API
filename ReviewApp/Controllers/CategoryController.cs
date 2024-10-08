﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewApp.Dto;
using ReviewApp.Interfaces;
using ReviewApp.Models;
using ReviewApp.Repository;

namespace ReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

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
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories()); // map to dto

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(categories);
        }

        // get category by id
        [HttpGet("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId)); // map to dto

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(category);
        }

        // get pokemons by category id
        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Pokemon>))]

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPokemonByCategoryId(int categoryId)
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonBycategory(categoryId)); // map model to dto

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemons);

        }

        // create category
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            try
            {
                if (categoryCreate == null)
                {
                    return BadRequest(ModelState);
                }

                // check if the category already exists
                var category = _categoryRepository.GetCategories()
                    .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (category != null)
                {
                    ModelState.AddModelError("", "Category already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var categoryMap = _mapper.Map<Category>(categoryCreate); // map dto to model

                if (!_categoryRepository.CreateCategory(categoryMap))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong while creating the category");
            }
        }

        // update category
        [HttpPut("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updateCategory)
        {
            try
            {
                if (updateCategory == null)
                {
                    return BadRequest(ModelState);
                }

                if (categoryId != updateCategory.Id)
                {
                    return BadRequest(ModelState);
                }

                if (!_categoryRepository.CategoryExists(categoryId))
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var categoryMap = _mapper.Map<Category>(updateCategory); // map dto to model

                if (!_categoryRepository.updateCategory(categoryMap))
                {
                    ModelState.AddModelError(" ", "Something went wrong updating category");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return Ok("Success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the category.");
            }
        }

        // delete category
        [HttpDelete("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteCategory(int categoryId)
        {
            try
            {
                if (!_categoryRepository.CategoryExists(categoryId))
                {
                    return NotFound();
                }

                var categoryToDelete = _categoryRepository.GetCategory(categoryId);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!_categoryRepository.DetachCategory(categoryToDelete))
                {
                    ModelState.AddModelError("", "Something went wrong deleting category");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the category.");
            }

        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewApp.Dto;
using ReviewApp.Interfaces;
using ReviewApp.Models;
using ReviewApp.Repository;

namespace ReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, IReviewRepository reviewRepository, IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        // get all pokemons
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PokemonDto>))]
        public IActionResult GetPokemons()
        {
            try
            {
                var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());
                return Ok(pokemons);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving pokemons");
            }
        }

        // get pokemon by id
        [HttpGet("{pokeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PokemonDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPokemon(int pokeId)
        {
            try
            {
                if (!_pokemonRepository.PokemonExists(pokeId))
                {
                    return NotFound();
                }

                var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
                return Ok(pokemon);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving pokemon");
            }
        }

        // get pokemon rating by id
        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(float))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            try
            {
                if (!_pokemonRepository.PokemonExists(pokeId))
                {
                    return NotFound();
                }

                var rating = _pokemonRepository.GetPokemonRating(pokeId);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving pokemon rating");
            }
        }

        // create pokemon
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto pokemonCreate)
        {
            try
            {
                if (pokemonCreate == null)
                {
                    return BadRequest("Invalid input");
                }

                var pokemons = _pokemonRepository.GetPokemonTrimToUpper(pokemonCreate);


                if (pokemons != null)
                {
                    ModelState.AddModelError("", "Pokemon already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
                }

                var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

                if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap))
                {
                    ModelState.AddModelError("", "Error saving pokemon");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating pokemon");
            }
        }

        // update pokemon
        [HttpPut("{pokeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePokemon(int pokeId, [FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto updatePokemon)
        {
            try
            {
                if (updatePokemon == null)
                {
                    return BadRequest("Invalid input");
                }

                if (pokeId != updatePokemon.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!_pokemonRepository.PokemonExists(pokeId))
                {
                    return NotFound();
                }

                var pokemonMap = _mapper.Map<Pokemon>(updatePokemon);

                if (!_pokemonRepository.UpdatePokemon(ownerId, catId, pokemonMap))
                {
                    ModelState.AddModelError("", "Error updating pokemon");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating pokemon");
            }
        }

        // delete pokemon
        [HttpDelete("{pokeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeletePokemon(int pokeId)
        {
            try
            {
                if (!_pokemonRepository.PokemonExists(pokeId))
                {
                    return NotFound();
                }

                var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId);
                var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);

                if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
                {
                    ModelState.AddModelError("", "Error deleting reviews");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
                {
                    ModelState.AddModelError("", "Error deleting pokemon");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting pokemon");
            }
        }
    }
}
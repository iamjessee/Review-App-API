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
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository; // repo for data access
        private readonly IMapper _mapper; // automapper for DTO conversion

        // constructor for injecting dependencies
        public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
        }

        // get all pokemons
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons()); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(pokemons); // return pokemons
        }

        // get pokemon by id
        [HttpGet("{pokeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pokemon))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound(); // return not found if pokemon does not exist
            }

            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(pokemon); // return pokemon
        }

        // get pokemon rating by id
        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pokemon))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound(); // return not found if pokemon does not exist
            }

            var rating = _pokemonRepository.GetPokemonRating(pokeId); // get rating

            if (!ModelState.IsValid)
            {
                return BadRequest(); // return bad request if model state is invalid
            }

            return Ok(rating); // return rating
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto pokemonCreate)
        {
            try
            {
                if (pokemonCreate == null)
                {
                    return BadRequest(ModelState); // return bad request if input is null
                }

                // check if the pokemon already exists
                var pokemons = _pokemonRepository.GetPokemons()
                    .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (pokemons != null)
                {
                    ModelState.AddModelError("", "Pokemon already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState); // return conflict if owner exists
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // return bad request if model state is invalid
                }

                var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate); // map dto to model

                if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState); // return server error if save fails
                }

                return Ok("Successfully created"); // return success message
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong while creating the pokemon"); // return server error
            }
        }
    }
}
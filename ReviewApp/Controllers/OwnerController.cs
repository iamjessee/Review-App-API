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
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        // get all owners
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OwnerDto>))]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners()); // map dto to owner
            return Ok(owners);
        }

        // get owner by id
        [HttpGet("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OwnerDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));
            return Ok(owner);
        }

        // get pokemon by owner
        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PokemonDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var pokemons = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));
            return Ok(pokemons);
        }

        // create owner
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
        {
            try
            {
                if (ownerCreate == null)
                {
                    return BadRequest("Invalid input");
                }

                var existingOwner = _ownerRepository.GetOwners()
                    .FirstOrDefault(c => c.LastName.Trim().ToUpper() == ownerCreate.LastName.TrimEnd().ToUpper());

                if (existingOwner != null)
                {
                    ModelState.AddModelError("", "Owner already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
                }

                var ownerMap = _mapper.Map<Owner>(ownerCreate);
                ownerMap.Country = _countryRepository.GetCountry(countryId);

                if (!_ownerRepository.CreateOwner(ownerMap))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error saving owner");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating owner");
            }
        }

        // update owner
        [HttpPut("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updateOwner)
        {
            try
            {
                if (updateOwner == null)
                {
                    return BadRequest("Invalid input");
                }

                if (ownerId != updateOwner.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!_ownerRepository.OwnerExists(ownerId))
                {
                    return NotFound();
                }

                var ownerMap = _mapper.Map<Owner>(updateOwner);

                if (!_ownerRepository.UpdateOwner(ownerMap))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error updating owner");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating owner");
            }
        }

        // delete owner
        [HttpDelete("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteOwner(int ownerId)
        {
            try
            {
                if (!_ownerRepository.OwnerExists(ownerId))
                {
                    return NotFound();
                }

                var ownerToDelete = _ownerRepository.GetOwner(ownerId);

                if (!_ownerRepository.DeleteOwner(ownerToDelete))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting owner");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting owner");
            }
        }
    }
}
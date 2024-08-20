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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners()); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(owners); // return owners
        }

        // get owner by id
        [HttpGet("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pokemon))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound(); // return not found if owner does not exist
            }

            var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(owner); // return owner
        }

        // get pokemon by owner
        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pokemon))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound(); // return not found if owner does not exist
            }
            var owner = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(owner); // return owner
        }

        // create owner
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
        {
            try
            {
                if (ownerCreate == null)
                {
                    return BadRequest(ModelState); // return bad request if input is null
                }

                // check if the owner already exists
                var owners = _ownerRepository.GetOwners()
                    .Where(c => c.LastName.Trim().ToUpper() == ownerCreate.LastName.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (owners != null)
                {
                    ModelState.AddModelError("", "Owner already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState); // return conflict if owner exists
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // return bad request if model state is invalid
                }

                var ownerMap = _mapper.Map<Owner>(ownerCreate); // map dto to model

                ownerMap.Country = _countryRepository.GetCountry(countryId); // assign country to owner

                if (!_ownerRepository.CreateOwner(ownerMap))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState); // return server error if save fails
                }

                return Ok("Successfully created"); // return success message
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong while creating the owner"); // return server error
            }
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updateOwner)
        {
            if (updateOwner == null)
            {
                return BadRequest(ModelState);
            }

            if (ownerId != updateOwner.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var ownerMap = _mapper.Map<Owner>(updateOwner);

            if (!_ownerRepository.updateOwner(ownerMap))
            {
                ModelState.AddModelError(" ", "Something went wrong updating owner");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok("Success!");
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewApp.Dto;
using ReviewApp.Interfaces;
using ReviewApp.Models;

namespace ReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        // get all countries
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries()); // map dto to model

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(countries);
        }

        // get country by id
        [HttpGet("{countryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CountryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }

            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId)); // map dto to model

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(country);
        }

        // get country by owner id
        [HttpGet("/owners/{ownerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CountryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetCountryOfAnOwner(int ownerId)
        {
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(country);
        }

        // create country
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            try
            {
                if (countryCreate == null)
                {
                    return BadRequest(ModelState);
                }

                var country = _countryRepository.GetCountries()
                    .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (country != null)
                {
                    ModelState.AddModelError("", "country already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var countryMap = _mapper.Map<Country>(countryCreate); // map model to dto

                if (!_countryRepository.CreateCountry(countryMap))
                {
                    ModelState.AddModelError("", "something went wrong while saving");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return Ok("successfully created");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "something went wrong while creating the country");
            }
        }

        // update country
        [HttpPut("{countryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto updateCountry)
        {
            try
            {
                if (updateCountry == null)
                {
                    return BadRequest(ModelState);
                }

                if (countryId != updateCountry.Id)
                {
                    return BadRequest(ModelState);
                }

                if (!_countryRepository.CountryExists(countryId))
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var countryMap = _mapper.Map<Country>(updateCountry); // map model to dto

                if (!_countryRepository.updateCountry(countryMap))
                {
                    ModelState.AddModelError(" ", "something went wrong while updating country");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return Ok("successfully updated");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "something went wrong while updating the country");
            }
        }

        // delete country by id
        [HttpDelete("{countryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteCountry(int countryId)
        {
            try
            {
                if (!_countryRepository.CountryExists(countryId))
                {
                    return NotFound();
                }

                var countryToDelete = _countryRepository.GetCountry(countryId);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!_countryRepository.DeleteCountry(countryToDelete))
                {
                    ModelState.AddModelError("", "something went wrong while deleting country");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "something went wrong while deleting the country");
            }
        }
    }
}
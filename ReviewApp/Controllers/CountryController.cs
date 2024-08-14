using AutoMapper;
using Azure.Core;
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

    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryrepository; // repo for country data access
        private readonly IMapper _mapper; // automapper for DTO conversion

        // constructor for injecting dependencies
        public CountryController(ICountryRepository countryrepository, IMapper mapper)
        {
            _countryrepository = countryrepository;
            _mapper = mapper;
        }

        // get all countries
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryrepository.GetCountries()); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(countries); // return countries
        }

        // get country by id
        [HttpGet("{countryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Country))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryrepository.CountryExists(countryId))
            {
                return NotFound(); // return not found if country does not exist
            }

            var country = _mapper.Map<CountryDto>(_countryrepository.GetCountry(countryId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(country); // return country
        }

        // get country by owner id
        [HttpGet("/owners/{ownerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Country))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetCountryOfAnOwner(int ownerId)
        {
            var country = _mapper.Map<CountryDto>(_countryrepository.GetCountryByOwner(ownerId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(country); // return country
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            try
            {
                if (countryCreate == null)
                {
                    return BadRequest(ModelState); // return bad request if input is null
                }

                // check if the country already exists
                var country = _countryrepository.GetCountries()
                    .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (country != null)
                {
                    ModelState.AddModelError("", "country already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState); // return conflict if country exists
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // return bad request if model state is invalid
                }

                var countryMap = _mapper.Map<Country>(countryCreate); // map dto to model

                if (!_countryrepository.CreateCountry(countryMap))
                {
                    ModelState.AddModelError("", "something went wrong while saving");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState); // return server error if save fails
                }

                return Ok("successfully created"); // return success message
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "something went wrong while creating the country"); // return server error
            }
        }
    }
}
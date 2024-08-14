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
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository; // repo for data access
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper; // automapper for DTO conversion

        // constructor for injecting dependencies
        public ReviewController(IReviewRepository reviewRepository, IPokemonRepository pokemonRepository, IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        // get all reviews
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews()); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(reviews); // return pokemons
        }

        // get review by id
        [HttpGet("{reviewId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Review))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound(); // return not found if review does not exist
            }

            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(review); // return review
        }

        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Review))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetReviewsForAPokemon(int pokeId)
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // return bad request if model state is invalid
            }
            return Ok(reviews); // return reviews
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId, [FromBody] ReviewDto reviewCreate)
        {
            try
            {
                if (reviewCreate == null)
                {
                    return BadRequest(ModelState); // return bad request if input is null
                }

                // check if the review already exists
                var reviews = _reviewRepository.GetReviews()
                    .Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (reviews != null)
                {
                    ModelState.AddModelError("", "review already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState); // return conflict if review exists
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // return bad request if model state is invalid
                }

                var reviewMap = _mapper.Map<Review>(reviewCreate); // map dto to model

                reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId); // assign pokemon to review
                reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId); // assign reviewer to review

                if (!_reviewRepository.CreateReview(reviewMap))
                {
                    ModelState.AddModelError("", "something went wrong while saving");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState); // return server error if save fails
                }

                return Ok("successfully created"); // return success message
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "something went wrong while creating the review"); // return server error
            }
        }
    }
}
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
    public class ReviewController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, IPokemonRepository pokemonRepository, IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        // get all reviews
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviews()
        {
            try
            {
                var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving reviews");
            }
        }

        // get review by id
        [HttpGet("{reviewId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetReview(int reviewId)
        {
            try
            {
                if (!_reviewRepository.ReviewExists(reviewId))
                {
                    return NotFound();
                }

                var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));
                return Ok(review);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving review");
            }
        }

        // get reviews by pokemon id
        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetReviewsForAPokemon(int pokeId)
        {
            try
            {
                var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving reviews for Pokémon");
            }
        }

        // create review
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId, [FromBody] ReviewDto reviewCreate)
        {
            try
            {
                if (reviewCreate == null)
                {
                    return BadRequest("Invalid input");
                }

                var existingReview = _reviewRepository.GetReviews()
                    .FirstOrDefault(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper());

                if (existingReview != null)
                {
                    ModelState.AddModelError("", "Review already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
                }

                var reviewMap = _mapper.Map<Review>(reviewCreate);
                reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);
                reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);

                if (!_reviewRepository.CreateReview(reviewMap))
                {
                    ModelState.AddModelError("", "Error saving review");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating review");
            }
        }

        // update review
        [HttpPut("{reviewId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updateReview)
        {
            try
            {
                if (updateReview == null)
                {
                    return BadRequest("Invalid input");
                }

                if (reviewId != updateReview.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!_reviewRepository.ReviewExists(reviewId))
                {
                    return NotFound();
                }

                var reviewMap = _mapper.Map<Review>(updateReview);

                if (!_reviewRepository.UpdateReview(reviewMap))
                {
                    ModelState.AddModelError("", "Error updating review");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating review");
            }
        }

        // delete review
        [HttpDelete("{reviewId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteReview(int reviewId)
        {
            try
            {
                if (!_reviewRepository.ReviewExists(reviewId))
                {
                    return NotFound();
                }

                var reviewToDelete = _reviewRepository.GetReview(reviewId);

                if (!_reviewRepository.DeleteReview(reviewToDelete))
                {
                    ModelState.AddModelError("", "Error deleting review");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting review");
            }
        }
    }
}
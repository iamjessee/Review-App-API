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
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository; // repo for data access
        private readonly IMapper _mapper; // automapper for DTO conversion

        // constructor for injecting dependencies
        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        // get all reviewers
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers()); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(reviewers); // return reviewers
        }

        // get reviewer by id
        [HttpGet("{reviewerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Reviewer))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound(); // return not found if reviewer does not exist
            }

            var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(reviewer); // return reviewer
        }

        [HttpGet ("{reviewerId}/reviews")]
        public IActionResult GetReviewsByAReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound(); // return not found if reviewer does not exist
            }
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId)); // map to DTO

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return bad request if model state is invalid
            }

            return Ok(reviews); // return reviewers
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
        {
            try
            {
                if (reviewerCreate == null)
                {
                    return BadRequest(ModelState); // return bad request if input is null
                }

                // check if the reviewer already exists
                var country = _reviewerRepository.GetReviewers()
                    .Where(c => c.LastName.Trim().ToUpper() == reviewerCreate.LastName.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (country != null)
                {
                    ModelState.AddModelError("", "reviewer already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState); // return conflict if reviewer exists
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // return bad request if model state is invalid
                }

                var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate); // map dto to model

                if (!_reviewerRepository.CreateReviewer(reviewerMap))
                {
                    ModelState.AddModelError("", "something went wrong while saving");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState); // return server error if save fails
                }

                return Ok("successfully created"); // return success message
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "something went wrong while creating the reviewer"); // return server error
            }
        }
    }
}
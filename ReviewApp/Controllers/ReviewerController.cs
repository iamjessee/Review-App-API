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
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        // get all reviewers
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetReviewers()
        {
            try
            {
                var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());
                return Ok(reviewers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving reviewers");
            }
        }

        // get reviewer by id
        [HttpGet("{reviewerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewerDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetReviewer(int reviewerId)
        {
            try
            {
                if (!_reviewerRepository.ReviewerExists(reviewerId))
                {
                    return NotFound();
                }

                var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));
                return Ok(reviewer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving reviewer");
            }
        }

        // get reviews by reviewer id
        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetReviewsByAReviewer(int reviewerId)
        {
            try
            {
                if (!_reviewerRepository.ReviewerExists(reviewerId))
                {
                    return NotFound();
                }

                var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving reviews");
            }
        }

        // create reviewer
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
        {
            try
            {
                if (reviewerCreate == null)
                {
                    return BadRequest("Invalid input");
                }

                var existingReviewer = _reviewerRepository.GetReviewers()
                    .FirstOrDefault(c => c.LastName.Trim().ToUpper() == reviewerCreate.LastName.TrimEnd().ToUpper());

                if (existingReviewer != null)
                {
                    ModelState.AddModelError("", "Reviewer already exists");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
                }

                var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);

                if (!_reviewerRepository.CreateReviewer(reviewerMap))
                {
                    ModelState.AddModelError("", "Error saving reviewer");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating reviewer");
            }
        }

        // update reviewer
        [HttpPut("{reviewerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updateReviewer)
        {
            try
            {
                if (updateReviewer == null)
                {
                    return BadRequest("Invalid input");
                }

                if (reviewerId != updateReviewer.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!_reviewerRepository.ReviewerExists(reviewerId))
                {
                    return NotFound();
                }

                var reviewerMap = _mapper.Map<Reviewer>(updateReviewer);

                if (!_reviewerRepository.UpdateReviewer(reviewerMap))
                {
                    ModelState.AddModelError("", "Error updating reviewer");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating reviewer");
            }
        }

        // delete reviewer
        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            try
            {
                if (!_reviewerRepository.ReviewerExists(reviewerId))
                {
                    return NotFound();
                }

                var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);

                if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
                {
                    ModelState.AddModelError("", "Error deleting reviewer");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting reviewer");
            }
        }
    }
}
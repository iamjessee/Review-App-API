using AutoMapper;
using ReviewApp.Data;
using ReviewApp.Interfaces;
using ReviewApp.Models;

namespace ReviewApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReviewRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool CreateReview(Review review)
        {
            _context.Add(review); // add the review to the context
            return Save(); // save changes and return true if successful
        }

        public Review GetReview(int reviewId)
        {
            return _context.Reviews
                .Where(r => r.Id == reviewId) // filter reviews by ID
                .FirstOrDefault(); // return the review if found, otherwise null
        }

        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.ToList(); // return a list of all reviews
        }

        public ICollection<Review> GetReviewsOfAPokemon(int pokeId)
        {
            return _context.Reviews
                .Where(r => r.Pokemon.Id == pokeId) // filter reviews by Pokemon ID
                .ToList(); // return a list of reviews for the specified Pokemon
        }

        public bool ReviewExists(int reviewId)
        {
            return _context.Reviews
                .Any(r => r.Id == reviewId); // check if a review with the given ID exists
        }

        public bool Save()
        {
            var saved = _context.SaveChanges(); // save changes and get the number of affected rows
            return saved > 0; // return true if any rows were affected, otherwise false
        }

        public bool UpdateReview(Review review)
        {
            _context.Update(review);
            return Save();
        }
    }
}
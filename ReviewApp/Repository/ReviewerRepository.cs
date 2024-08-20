using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReviewApp.Data;
using ReviewApp.Interfaces;
using ReviewApp.Models;

namespace ReviewApp.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReviewerRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            _context.Add(reviewer); // add the reviewer to the context
            return Save(); // save changes to the database and return true if successful
        }

        public Reviewer GetReviewer(int reviewerId)
        {
            return _context.Reviewers
                .Where(r => r.Id == reviewerId) // filter reviewers by ID
                .Include(e => e.Reviews) // include related reviews
                .FirstOrDefault(); // return the reviewer if found, otherwise null
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return _context.Reviewers.ToList(); // return a list of all reviewers
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return _context.Reviews
                .Where(r => r.Reviewer.Id == reviewerId) // filter reviews by reviewer ID
                .ToList(); // return a list of reviews by the specified reviewer
        }

        public bool ReviewerExists(int reviewerId)
        {
            return _context.Reviewers
                .Any(r => r.Id == reviewerId); // check if a reviewer with the given ID exists
        }

        public bool Save()
        {
            var saved = _context.SaveChanges(); // save changes and get the number of affected rows
            return saved > 0; // return true if any rows were affected, otherwise false
        }

        public bool updateReviewer(Reviewer reviewer)
        {
            _context.Update(reviewer);
            return Save();
        }
    }
}
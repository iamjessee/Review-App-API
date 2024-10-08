﻿using AutoMapper;
using ReviewApp.Data;
using ReviewApp.Interfaces;
using ReviewApp.Models;

namespace ReviewApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReviewRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateReview(Review review)
        {
            _context.Add(review); // add the review to the context
            return Save(); // save changes and return true if successful
        }

        public bool DeleteReview(Review review)
        {
            _context.Remove(review);
            return Save();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            _context.RemoveRange(reviews);
            return Save();
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
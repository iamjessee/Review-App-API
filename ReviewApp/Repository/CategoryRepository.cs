using ReviewApp.Data;
using ReviewApp.Interfaces;
using ReviewApp.Models;

namespace ReviewApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        public CategoryRepository(DataContext context)
        {
            _context = context;
        }
        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id); // check if a category with the given id exists
        }

        public bool CreateCategory(Category category)
        {

            _context.Add(category); // add the new category to the context

            return Save(); // save changes and return true if successful
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.ToList(); // retrieve and return a list of all categories
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Where(e => e.Id == id).FirstOrDefault(); // return the category if found, otherwise null
        }

        public ICollection<Pokemon> GetPokemonBycategory(int categoryId)
        {
            return _context.PokemonCategories
                .Where(e => e.CategoryId == categoryId) // filter by category id
                .Select(c => c.Pokemon) // select related pokemon
                .ToList(); // return as a list
        }

        public bool Save()
        {
            var saved = _context.SaveChanges(); // save changes and get the number of affected rows
            return saved > 0 ? true : false; // return true if any rows were affected, otherwise false

        }

        public bool updateCategory(Category category)
        {
            _context.Update(category);
            return Save();
        }
    }
}
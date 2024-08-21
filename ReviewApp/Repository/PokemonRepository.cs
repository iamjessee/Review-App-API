using ReviewApp.Data;
using ReviewApp.Interfaces;
using ReviewApp.Models;
using System.Reflection.Metadata.Ecma335;

namespace ReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;

        public PokemonRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners
                .Where(a => a.Id == ownerId) // find the owner by ID
                .FirstOrDefault();

            var category = _context.Categories
                .Where(a => a.Id == categoryId) // find the category by ID
                .FirstOrDefault();

            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity, // associate the Pokemon with the owner
                Pokemon = pokemon
            };

            _context.Add(pokemonOwner); // add the PokemonOwner to the context

            var pokemonCategory = new PokemonCategory()
            {
                Category = category, // associate the Pokemon with the category
                Pokemon = pokemon
            };

            _context.Add(pokemonCategory); // add the PokemonCategory to the context

            _context.Add(pokemon); // add the Pokemon to the context

            return Save(); // save changes to the database and return true if successful
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon);
            return Save();
        }

        public Pokemon GetPokemon(int id)
        {
            return _context.Pokemon
                .Where(p => p.Id == id) // filter Pokemon by ID
                .FirstOrDefault(); // return the Pokemon if found, otherwise null
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemon
                .Where(p => p.Name == name) // filter Pokemon by name
                .FirstOrDefault(); // return the Pokemon if found, otherwise null
        }

        public decimal GetPokemonRating(int pokeId)
        {
            var review = _context.Reviews
                .Where(p => p.Pokemon.Id == pokeId); // filter Reviews by Pokemon ID

            if (review.Count() <= 0)
            {
                return 0; // return 0 if no reviews are found
            }

            return ((decimal)review.Sum(r => r.Rating) / review.Count()); // calculate and return the average rating
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemon
                .OrderBy(p => p.Id) // order Pokemon by ID
                .ToList(); // return the list of all Pokemon
        }

        public bool PokemonExists(int pokeId)
        {
            return _context.Pokemon
                .Any(p => p.Id == pokeId); // check if a Pokemon with the given ID exists
        }

        public bool Save()
        {
            var saved = _context.SaveChanges(); // save changes and get the number of affected rows
            return saved > 0; // return true if any rows were affected, otherwise false
        }

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            _context.Update(pokemon);
            return Save();
        }
    }
}
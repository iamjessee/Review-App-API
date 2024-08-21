using ReviewApp.Data;
using ReviewApp.Interfaces;
using ReviewApp.Models;

namespace ReviewApp.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext _context;

        public OwnerRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateOwner(Owner owner)
        {
            _context.Add(owner); // add the new owner to the context
            return Save(); // save changes to the database and return true if successful
        }

        public bool DeleteOwner(Owner owner)
        {
            _context.Remove(owner);
            return Save();
        }

        public Owner GetOwner(int ownerId)
        {
            return _context.Owners
                .Where(o => o.Id == ownerId) // filter Owners by owner id
                .FirstOrDefault(); // return the owner if found, otherwise null
        }

        public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
        {
            return _context.PokemonOwners
                .Where(p => p.Pokemon.Id == pokeId) // filter PokemonOwners by pokemon id
                .Select(o => o.Owner) // select the related Owners
                .ToList(); // return the list of Owners
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.ToList(); // retrieve and return a list of all owners
        }

        public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
        {
            return _context.PokemonOwners
                .Where(p => p.Owner.Id == ownerId) // filter PokemonOwners by owner id
                .Select(p => p.Pokemon) // select the related Pokemon
                .ToList(); // return the list of Pokemon
        }

        public bool OwnerExists(int ownerId)
        {
            return _context.Owners.Any(o => o.Id == ownerId); // check if an owner with the given id exists
        }

        public bool Save()
        {
            var saved = _context.SaveChanges(); // save changes and return the number of affected rows
            return saved > 0; // return true if any rows were affected, otherwise false
        }

        public bool updateOwner(Owner owner)
        {
            _context.Update(owner);
            return Save();
        }
    }
}
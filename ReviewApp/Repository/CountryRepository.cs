using AutoMapper;
using ReviewApp.Data;
using ReviewApp.Interfaces;
using ReviewApp.Models;

namespace ReviewApp.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CountryRepository(DataContext context)
        {
            _context = context;
        }

        public bool CountryExists(int id)
        {
            return _context.Countries.Any(c => c.Id == id); // check if a country with the given id exists
        }

        public bool CreateCountry(Country country)
        {
            _context.Add(country); // add the new country to the context
            return Save(); // save changes to the database and return true if successful
        }

        public bool DeleteCountry(Country country)
        {
            _context.Remove(country);
            return Save();
        }

        public ICollection<Country> GetCountries()
        {
            return _context.Countries.ToList(); // retrieve and return a list of all countries
        }

        public Country GetCountry(int id)
        {
            return _context.Countries.Where(c => c.Id == id).FirstOrDefault(); // retrieve and return the country by id, or null if not found
        }

        public Country GetCountryByOwner(int ownerId)
        {
            return _context.Owners
                .Where(o => o.Id == ownerId) // filter Owners by owner id
                .Select(c => c.Country) // select the related Country
                .FirstOrDefault(); // return the country if found, otherwise null
        }

        public ICollection<Owner> GetOwnerFromACountry(int countryId)
        {
            return _context.Owners
                .Where(c => c.Country.Id == countryId) // filter Owners by country id
                .ToList(); // return the list of Owners
        }

        public bool Save()
        {
            var saved = _context.SaveChanges(); // save changes and return the number of affected rows
            return saved > 0; // return true if any rows were affected, otherwise false
        }

        public bool updateCountry(Country country)
        {
            _context.Update(country);
            return Save();
        }
    }
}
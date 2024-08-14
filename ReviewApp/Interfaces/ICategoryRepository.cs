using ReviewApp.Models;

namespace ReviewApp.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int id);
        ICollection<Pokemon> GetPokemonBycategory(int categoryId);
        bool CategoryExists(int id);
        bool CreateCategory(Category category);
        bool Save();
    }
}

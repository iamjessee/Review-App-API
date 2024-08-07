using Microsoft.EntityFrameworkCore;
using ReviewApp.Models;

namespace ReviewApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            // constructor to pass options to the base DbContext class
        }

        // define DbSets for each entity type
        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Pokemon> Pokemon { get; set; }
        public DbSet<PokemonOwner> PokemonOwners { get; set; }
        public DbSet<PokemonCategory> PokemonCategories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configure composite primary key for PokemonCategory
            modelBuilder.Entity<PokemonCategory>()
                .HasKey(pc => new { pc.PokemonId, pc.CategoryId });

            // configure relationships for PokemonCategory
            modelBuilder.Entity<PokemonCategory>()
                .HasOne(p => p.Pokemon) // each PokemonCategory has one Pokemon
                .WithMany(pc => pc.PokemonCategories) // each Pokemon can have many PokemonCategories
                .HasForeignKey(p => p.PokemonId); // foreign key in PokemonCategory referencing Pokemon

            modelBuilder.Entity<PokemonCategory>()
                .HasOne(c => c.Category) // each PokemonCategory has one Category
                .WithMany(pc => pc.PokemonCategories) // each Category can have many PokemonCategories
                .HasForeignKey(c => c.CategoryId); // foreign key in PokemonCategory referencing Category

            // configure composite primary key for PokemonOwner
            modelBuilder.Entity<PokemonOwner>()
                .HasKey(po => new { po.PokemonId, po.OwnerId });

            // configure relationships for PokemonOwner
            modelBuilder.Entity<PokemonOwner>()
                .HasOne(p => p.Pokemon) // each PokemonOwner has one Pokemon
                .WithMany(po => po.PokemonOwners) // each Pokemon can have many PokemonOwners
                .HasForeignKey(p => p.PokemonId); // foreign key in PokemonOwner referencing Pokemon

            modelBuilder.Entity<PokemonOwner>()
                .HasOne(o => o.Owner) // each PokemonOwner has one Owner
                .WithMany(po => po.PokemonOwners) // each Owner can have many PokemonOwners
                .HasForeignKey(o => o.OwnerId); // foreign key in PokemonOwner referencing Owner
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace imovi_backend.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<FavouriteMovie> FavoriteMovies { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<UserMovieHistory> UserMovieHistories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

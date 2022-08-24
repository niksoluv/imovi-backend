using Microsoft.EntityFrameworkCore;

namespace imovi_backend.Models
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public DbSet<FavouriteMovie> FavoriteMovies { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public DbSet<UserMovieHistory> UserMovieHistories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<LikedComment> LikedComments { get; set; }
        public virtual DbSet<CustomList> CustomLists { get; set; }
        public virtual DbSet<CustomListMovie> CustomListsMovies { get; set; }
        public DbSet<CommentReply> CommentReplies { get; set; }
        public ApplicationContext()
            : base()
        {
            
        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

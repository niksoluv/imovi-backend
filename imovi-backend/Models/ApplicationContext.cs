using Microsoft.EntityFrameworkCore;

namespace imovi_backend.Models
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<FavouriteMovie> FavoriteMovies { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<UserMovieHistory> UserMovieHistories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<LikedComment> LikedComments { get; set; }
        public virtual DbSet<CustomList> CustomLists { get; set; }
        public virtual DbSet<CustomListMovie> CustomListsMovies { get; set; }
        public virtual DbSet<CommentReply> CommentReplies { get; set; }
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

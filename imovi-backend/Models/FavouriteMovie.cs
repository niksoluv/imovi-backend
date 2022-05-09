using System;

namespace imovi_backend.Models
{
    public class FavouriteMovie : BaseEntity
    {
        public Guid MovieId { get; set; }
        public Guid UserId { get; set; }
        public Movie Movie { get; set; }
    }
}

using System;

namespace imovi_backend.Models
{
    public class UserMovieHistory : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid MovieId { get; set; }
    }
}

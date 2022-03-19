using System;

namespace imovi_backend.Models
{
    public class FavouriteMovie : BaseEntity
    {
        public string MovieId { get; set; }
        public Guid UserId { get; set; }
        public string MediaType { get; set; }
    }
}

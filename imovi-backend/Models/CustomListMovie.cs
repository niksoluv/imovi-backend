using System;

namespace imovi_backend.Models
{
    public class CustomListMovie : BaseEntity
    {
        public Guid MovieId { get; set; }
        public Guid CustomListId { get; set; }
        public Movie Movie { get; set; }
    }
}

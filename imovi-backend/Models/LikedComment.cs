using System;

namespace imovi_backend.Models
{
    public class LikedComment : BaseEntity
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get;set;}
    }
}

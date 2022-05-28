using System;
using System.Collections.Generic;

namespace imovi_backend.Models
{
    public class CommentReply : BaseEntity
    {
        public string Data { get;set;}
        public Guid CommentId { get;set;}
        public User User { get; set; }
        public bool IsEdited { get; set; } = false;
        public int Likes { get; set; } = 0;
        public IEnumerable<LikedComment> UsersLikes { get; set; }
    }
}

using System.Collections.Generic;

namespace imovi_backend.Models
{
    public class Comment : BaseEntity
    {
        public string Data { get;set; }
        public User User { get;set; }
        public bool IsEdited { get;set; } = false;
        public int Likes { get;set;} = 0;
        public int Dislikes { get;set;} = 0;
        public Movie Movie { get;set; }
        public IEnumerable<LikedComment> UsersLikes { get;set;}
    }
}

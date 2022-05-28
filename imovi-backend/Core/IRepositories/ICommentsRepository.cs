using imovi_backend.Data;
using imovi_backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace imovi_backend.Core.IRepositories
{
    public interface ICommentsRepository : IGenericRepository<Comment>
    {
        Task<Comment> CreateComment(Comment comment, User user);
        Task<Comment> ReplyComment(CommentReplyDTO commentReply, User user);
        Task<List<Comment>> GetMovieComments(string movieId);
        Task<Comment>LikeComment(Guid commentId, Guid userId);
        Task<Comment> UnlikeComment(Guid commentId, Guid userId);
    }
}

using imovi_backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace imovi_backend.Core.IRepositories
{
    public interface ICommentsRepository : IGenericRepository<Comment>
    {
        Task<Comment> CreateComment(Comment comment, User user);
        Task<List<Comment>> GetMovieComments(string movieId);
    }
}

using imovi_backend.Core.IRepositories;
using System.Threading.Tasks;

namespace imovi_backend.Core.IConfiguration
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IMoviesRepository Movies { get; }
        IMovieHistoryRepository UserHistory { get; }
        ICommentsRepository Comments { get; }
        Task CompleteAsync();
    }
}

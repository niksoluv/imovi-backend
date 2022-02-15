using imovi_backend.Core.IRepositories;
using System.Threading.Tasks;

namespace imovi_backend.Core.IConfiguration
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        Task CompleteAsync();
    }
}

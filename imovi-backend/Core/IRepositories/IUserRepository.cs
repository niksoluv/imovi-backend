using imovi_backend.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace imovi_backend.Core.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<string> GetUserUsername(Guid id);
        object GetToken(User user);
    }
}

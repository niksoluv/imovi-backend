using imovi_backend.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace imovi_backend.Core.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<string> GetUserUsername(Guid id);
        Task<User> GetByUsername(string username);
        object GetToken(User user);
        Task<bool> DoesUsernameExists(string username);
        Task<bool> DoesEmailExists(string email);
    }
}

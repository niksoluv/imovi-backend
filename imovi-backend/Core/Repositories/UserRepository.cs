using imovi_backend.Core.IRepositories;
using imovi_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace imovi_backend.Core.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(
            ApplicationContext context,
            ILogger logger) : base(context, logger)
        {

        }

        public override async Task<IEnumerable<User>> All()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method error", typeof(UserRepository));
                return new List<User>();
            }
        }

        public async Task<string> GetUserUsername(Guid id)
        {
            try
            {
                var user = await dbSet.FirstOrDefaultAsync(x => x.Id == id);
                if(user != null)
                return user.Username;

                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetUserUsername method error", typeof(UserRepository));
                return "";
            }
        }

        public override async Task<bool> Upsert(User user)
        {
            try
            {
                var existingUser = await dbSet.Where(u => u.Id == user.Id).FirstOrDefaultAsync();
                if (existingUser == null)
                    return await Add(user);

                existingUser.Username = user.Username;
                existingUser.Email = user.Email;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Upsert method error", typeof(UserRepository));
                return false;
            }
        }

        public override async Task<bool> Delete(Guid id)
        {
            try
            {
                var existingUser = await dbSet.Where(u => u.Id == id).FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    dbSet.Remove(existingUser);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Delete} All method error", typeof(UserRepository));
                return false;
            }
        }
    }
}

using imovi_backend.Core.IRepositories;
using imovi_backend.Models;
using imovi_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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

        public override async Task<IEnumerable<User>> All(Guid userId)
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
                if (user != null)
                    return user.Username;

                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetUserUsername method error", typeof(UserRepository));
                return "";
            }
        }

        public async Task<User> GetByUsername(string username)
        {
            try
            {
                var user = await dbSet.FirstOrDefaultAsync(x => x.Username == username);
                if (user != null)
                    return user;

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetByUsername method error", typeof(UserRepository));
                return null;
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

        public object GetToken(User user)
        {
            var identity = GetIdentity(user.Username, user.Password);
            if (identity == null)
            {
                return null;
                //return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            return response;
        }
        private ClaimsIdentity GetIdentity(string username, string password)
        {
            User user = dbSet.FirstOrDefault(x => x.Username == username && x.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using DataModel.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _db;
        private readonly IdentityOptions _identityOptions;

        public UserService(DatabaseContext db, IOptions<IdentityOptions> identityOptions)
        {
            _db = db;
            _identityOptions = identityOptions.Value;
        }

        public async Task<TokenObject> Authenticate(string uniqueId, int expireHours = 24)
        {
            var user = await GetUser(uniqueId);

            if (user is null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var expires = DateTime.UtcNow.AddHours(expireHours);

            var key = Encoding.ASCII.GetBytes(_identityOptions.ApiSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var result = new TokenObject
            {
                Expiration = expires,
                Token = tokenString
            };

            return result;
        }

        private async Task<User> GetUser(string uniqueId)
        {
            var result = await _db.Users.FirstOrDefaultAsync(user => user.UniqueId == uniqueId);

            return result;
        }
    }
}

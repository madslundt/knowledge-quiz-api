using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using API.Infrastructure.Identity;
using DataModel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Features.User
{
    public class GetUserToken
    {
        public class Query : IRequest<Result>
        {
            public string UniqueId { get; set; }
            public TimeSpan TimeSpan { get; set; } = new TimeSpan(1, 0, 0, 0);
        }

        public class Result
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
        }

        public class GetUserValidator : AbstractValidator<Query>
        {
            public GetUserValidator()
            {
                RuleFor(query => query).NotNull();
                RuleFor(query => query.UniqueId).NotEmpty();
                RuleFor(query => query.TimeSpan).GreaterThanOrEqualTo(new TimeSpan(1, 0, 0)).LessThanOrEqualTo(new TimeSpan(7, 0, 0));
            }
        }


        public class GetUserHandler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;
            private readonly IdentityOptions _identityOptions;

            public GetUserHandler(DatabaseContext db, IOptions<IdentityOptions> identityOptions)
            {
                _db = db;
                _identityOptions = identityOptions.Value;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var user = await GetUser(message.UniqueId);

                if (user is null)
                {
                    throw new ArgumentNullException($"Could not find {nameof(message.UniqueId)} '{message.UniqueId}'");
                }

                var tokenHandler = new JwtSecurityTokenHandler();

                var expires = DateTime.UtcNow.Add(message.TimeSpan);

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

                var result = new Result
                {
                    Expiration = expires,
                    Token = tokenString
                };

                return result;
            }

            private async Task<DataModel.Models.User.User> GetUser(string uniqueId)
            {
                var result = await _db.Users.FirstOrDefaultAsync(user => user.UniqueId == uniqueId);

                return result;
            }
        }
    }
}

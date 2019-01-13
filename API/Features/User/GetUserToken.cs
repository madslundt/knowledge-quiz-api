using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.User
{
    public class GetUserToken
    {
        public class Query : IRequest<Result>
        {
            public string UniqueId { get; set; }
        }

        public class Result
        {
            public string Token { get; set; }
        }

        public class GetUserValidator : AbstractValidator<Query>
        {
            public GetUserValidator()
            {
                RuleFor(query => query).NotNull();
                RuleFor(query => query.UniqueId).NotEmpty();
            }
        }


        public class GetUserHandler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;

            public GetUserHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var userId = await Get(message.UniqueId);

                if (userId == Guid.Empty)
                {
                    throw new ArgumentNullException($"Could not find {nameof(message.UniqueId)} '{message.UniqueId}'");
                }

                var tokenHandler = new JwtSecurityTokenHandler();

                ClaimsIdentity identity = new ClaimsIdentity();
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString(), ClaimValueTypes.String));

                var securityToken = tokenHandler.CreateJwtSecurityToken(subject: identity);

                var token = tokenHandler.WriteToken(securityToken);

                var result = new Result
                {
                    Token = token
                };

                return result;
            }

            private async Task<Guid> Get(string uniqueId)
            {
                var query = from user in _db.Users
                    where user.UniqueId == uniqueId
                    select user.Id;

                var result = await query.FirstOrDefaultAsync();

                return result;
            }
        }
    }
}

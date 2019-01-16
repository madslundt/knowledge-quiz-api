using DataModel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Features.User
{
    public class GetUser
    {
        public class Query : IRequest<Result>
        {
            public Guid UserId { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }
        }

        public class GetUserValidator : AbstractValidator<Query>
        {
            public GetUserValidator()
            {
                RuleFor(query => query.UserId).NotEmpty();
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
                var result = await Get(message.UserId);

                if (result is null)
                {
                    throw new ArgumentNullException($"Could not find {nameof(message.UserId)} '{message.UserId}'");
                }

                return result;
            }

            private async Task<Result> Get(Guid userId)
            {
                var query = from user in _db.Users
                    where user.Id == userId
                    select new Result
                    {
                        Id = user.Id
                    };

                var result = await query.FirstOrDefaultAsync();

                return result;
            }
        }
    }
}

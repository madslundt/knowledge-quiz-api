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
    public class DoesUserExist
    {
        public class Query : IRequest<Result>
        {
            public Guid UserId { get; set; }
        }

        public class Result
        {
            public bool DoesUserExist { get; set; }
        }

        public class DoesUserExistValidator : AbstractValidator<Query>
        {
            public DoesUserExistValidator()
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
                var doesUserExist = await Get(message.UserId);

                var result = new Result
                {
                    DoesUserExist = doesUserExist
                };

                return result;
            }

            private async Task<bool> Get(Guid userId)
            {
                var query = from user in _db.Users
                            where user.Id == userId
                            select user.Id;

                var result = await query.AnyAsync();

                return result;
            }
        }
    }
}

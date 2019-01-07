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
    public class GetUserByUniqueId
    {
        public class Query : IRequest<Result>
        {
            public string UniqueId { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }
        }

        public class GetUserByUniqueIdValidator : AbstractValidator<Query>
        {
            public GetUserByUniqueIdValidator()
            {
                RuleFor(query => query).NotNull();
                RuleFor(user => user.UniqueId).NotEmpty();
            }
        }


        public class GetUserByUniqueIdHandler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;

            public GetUserByUniqueIdHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var result = await Get(message.UniqueId);

                if (result is null)
                {
                    throw new ArgumentNullException($"Could not find {nameof(message.UniqueId)} '{message.UniqueId}'");
                }

                return result;
            }

            private async Task<Result> Get(string uniqueId)
            {
                var query = from user in _db.Users
                            where user.UniqueId == uniqueId
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

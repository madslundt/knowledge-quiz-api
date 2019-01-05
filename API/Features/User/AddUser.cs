using DataModel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Features.User
{
    public class AddUser
    {
        public class Command : IRequest
        {
            public string UniqueId { get; set; }
        }

        public class AddUserValidator : AbstractValidator<Command>
        {
            public AddUserValidator()
            {
                RuleFor(metadata => metadata.UniqueId).NotEmpty();
            }
        }

        public class AddUserHandler : IRequestHandler<Command>
        {
            private readonly DatabaseContext _db;

            public AddUserHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var doesUserExist = await DoesUserExist(message.UniqueId);

                if (doesUserExist)
                {
                    return Unit.Value; // TODO
                }

                var user = PrepareUser(message.UniqueId); // TODO

                await _db.SaveChangesAsync();

                return Unit.Value;
            }

            private DataModel.Models.User.User PrepareUser(string uniqueId)
            {
                var user = new DataModel.Models.User.User
                {
                    UniqueId = uniqueId
                };

                _db.Users.Add(user);

                return user;
            }

            private async Task<bool> DoesUserExist(string uniqueId)
            {
                var query = from user in _db.Users
                            where user.UniqueId == uniqueId
                            select user.Id;

                var result = await query.AnyAsync();

                return result;
            }
        }
    }
}

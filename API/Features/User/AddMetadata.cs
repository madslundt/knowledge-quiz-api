using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Features.Answer;
using DataModel;
using DataModel.Models.User;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.User
{
    public class AddMetadata
    {
        public class Command : IRequest
        {
            public Guid UserId { get; set; }
            public ICollection<Metadata> Metadata { get; set; }
        }

        public class Metadata
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class AddMetadataValidator : AbstractValidator<Command>
        {
            public AddMetadataValidator()
            {
                RuleFor(metadata => metadata.UserId).NotEmpty();
                RuleFor(metadata => metadata.Metadata).NotEmpty();
                RuleFor(metadata => metadata.Metadata).Must(ContainAtleastOneValidType)
                    .WithMessage($"{nameof(Command.Metadata)} does not match metadata types");
            }

            private bool ContainAtleastOneValidType(ICollection<Metadata> metadata)
            {
                foreach (var m in metadata)
                {
                    if (Enum.TryParse(typeof(UserMetadataType), m.Key, true, out _) && !string.IsNullOrWhiteSpace(m.Value))
                    {
                        return true;
                    }
                }

                return false;
            }
        }


        public class AddAnswerHandler : IRequestHandler<Command>
        {
            private readonly DatabaseContext _db;
            private readonly IMediator _mediator;

            public AddAnswerHandler(DatabaseContext db, IMediator mediator)
            {
                _db = db;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var isUserExist = await IsUserExist(message.UserId);

                if (!isUserExist)
                {
                    throw new ArgumentNullException($"Could not find {nameof(message.UserId)} '{message.UserId}'");
                }

                var parsedMetadata = ParseMetadata(message.Metadata, message.UserId);
                _db.AddRange(parsedMetadata);
                await _db.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }

            private ICollection<UserMetadata> ParseMetadata(ICollection<Metadata> metadata, Guid userId)
            {
                var result = new List<UserMetadata>();
                foreach (var m in metadata)
                {
                    if (Enum.TryParse(m.Key, true, out UserMetadataType metadataType))
                    {
                        var userMetadata = new UserMetadata
                        {
                            MetadataType = metadataType,
                            UserId = userId,
                            Value = m.Value
                        };

                        result.Add(userMetadata);
                    }
                }

                return result;
            }

            private async Task<bool> IsUserExist(Guid userId)
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

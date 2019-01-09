using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            public Metadata Metadata { get; set; }
        }

        public class Metadata
        {
            public ICollection<MetadataItem> Metadatas { get; set; }
        }

        public class MetadataItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class AddMetadataValidator : AbstractValidator<Command>
        {
            public AddMetadataValidator()
            {
                RuleFor(command => command).NotNull();
                RuleFor(command => command.UserId).NotEmpty();
                RuleFor(command => command.Metadata).NotNull();
                RuleFor(command => command.Metadata.Metadatas).NotEmpty();
                RuleFor(command => command.Metadata.Metadatas).Must(ContainAtleastOneValidType)
                    .WithMessage($"{nameof(Command.Metadata)} does not match metadata types");
            }

            private bool ContainAtleastOneValidType(ICollection<MetadataItem> metadata)
            {
                foreach (var m in metadata)
                {
                    if (Enum.TryParse<UserMetadataType>(m.Key, true, out _) && !string.IsNullOrWhiteSpace(m.Value))
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

            public AddAnswerHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var parsedMetadata = ParseMetadata(message.Metadata.Metadatas, message.UserId);

                var metadataToAdd = await DistinctMetadata(message.UserId, parsedMetadata);

                await _db.AddRangeAsync(metadataToAdd);
                await _db.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }

            private async Task<ICollection<UserMetadata>> DistinctMetadata(Guid userId, ICollection<UserMetadata> parsedMetadata)
            {
                var query = from userMetadata in _db.UserMetadatas
                            join parsedUserMetadata in parsedMetadata on new { userMetadata.MetadataType, userMetadata.Value } equals new { parsedUserMetadata.MetadataType, parsedUserMetadata.Value }
                            where userMetadata.UserId == userId
                            select parsedUserMetadata;

                var metadata = await query.ToListAsync();

                var result = parsedMetadata.Except(metadata).ToList();

                return result;
            }

            private ICollection<UserMetadata> ParseMetadata(ICollection<MetadataItem> metadata, Guid userId)
            {
                var result = new List<UserMetadata>();
                foreach (var m in metadata)
                {
                    if (Enum.TryParse<UserMetadataType>(m.Key, true, out var metadataType))
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
        }
    }
}

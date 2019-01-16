using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Features.User;
using AutoFixture;
using DataModel.Models.User;
using FluentAssertions;
using FluentValidation;
using UnitTest.Common;
using Xunit;

namespace UnitTest.Features.User
{
    public class AddMetadataTest : TestBase
    {
        [Fact]
        public async Task ThrowArgumentNullExceptionWhenCommandIsNull()
        {
            var command = (AddMetadata.Command) null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsEmpty()
        {
            var command = _fixture.Build<AddMetadata.Command>()
                .With(x => x.UserId, Guid.Empty)
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsNull()
        {
            var command = _fixture.Build<AddMetadata.Command>()
                .Without(x => x.UserId)
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenMetadataIsEmpty()
        {
            var command = _fixture.Build<AddMetadata.Command>()
                .With(x => x.Metadata, new AddMetadata.Metadata
                {
                    Metadatas = new List<AddMetadata.MetadataItem>()
                })
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowNullReferenceExceptionWhenMetadataIsNull()
        {
            var command = _fixture.Build<AddMetadata.Command>()
                .Without(x => x.Metadata)
                .Create();

            await Assert.ThrowsAsync<NullReferenceException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task AddMetadataWhenItOnlyContainsValidTypes()
        {
            var metadatas = new List<AddMetadata.MetadataItem>();

            foreach (UserMetadataType metadataType in Enum.GetValues(typeof(UserMetadataType)))
            {
                metadatas.Add(new AddMetadata.MetadataItem
                {
                    Key = metadataType.ToString(),
                    Value = _fixture.Create<string>()
                });
            }

            var command = _fixture.Build<AddMetadata.Command>()
                .With(x => x.Metadata, new AddMetadata.Metadata
                {
                    Metadatas = metadatas
                })
                .Create();

            await _mediator.Send(command);

            var actualMetadatas = _db.UserMetadatas.ToList();
            actualMetadatas.Should().NotBeEmpty();
            actualMetadatas.Count.Should().Be(metadatas.Count);
            actualMetadatas.Should().OnlyContain(actualMetadata => actualMetadata.UserId == command.UserId);

            foreach (var metadata in metadatas)
            {
                actualMetadatas.Any(actualMetadata =>
                        actualMetadata.MetadataType.ToString() == metadata.Key &&
                        actualMetadata.Value == metadata.Value)
                    .Should().BeTrue();
            }
        }

        [Fact]
        public async Task AddOnlyValidMetadataWhenItContainsValidAndInvalidTypes()
        {
            var metadatas = new List<AddMetadata.MetadataItem>();

            foreach (UserMetadataType metadataType in Enum.GetValues(typeof(UserMetadataType)))
            {
                metadatas.Add(new AddMetadata.MetadataItem
                {
                    Key = metadataType.ToString(),
                    Value = _fixture.Create<string>()
                });
            }

            for (int i = 0; i < 10; i++)
            {
                metadatas.Add(new AddMetadata.MetadataItem
                {
                    Key = _fixture.Create<string>(),
                    Value = _fixture.Create<string>()
                });
            }

            var command = _fixture.Build<AddMetadata.Command>()
                .With(x => x.Metadata, new AddMetadata.Metadata
                {
                    Metadatas = metadatas
                })
                .Create();

            await _mediator.Send(command);

            var actualMetadatas = _db.UserMetadatas.ToList();
            actualMetadatas.Should().NotBeEmpty();
            actualMetadatas.Count.Should().Be(Enum.GetValues(typeof(UserMetadataType)).Length);
            actualMetadatas.Should().OnlyContain(actualMetadata => actualMetadata.UserId == command.UserId);

            foreach (var actualMetadata in actualMetadatas)
            {
                metadatas.Any(metadata =>
                        actualMetadata.MetadataType.ToString() == metadata.Key &&
                        actualMetadata.Value == metadata.Value)
                    .Should().BeTrue();
            }
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenMetadataOnlyContainsInvalidTypes()
        {
            var metadatas = new List<AddMetadata.MetadataItem>();

            for (int i = 0; i < 10; i++)
            {
                metadatas.Add(new AddMetadata.MetadataItem
                {
                    Value = _fixture.Create<string>()
                });
            }

            var command = _fixture.Build<AddMetadata.Command>()
                .With(x => x.Metadata, new AddMetadata.Metadata
                {
                    Metadatas = metadatas
                })
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task AddKeyMultipleTimesIfItIsIntroducedMultipleTimes()
        {
            // Since inmemory db does not check for foreign keys this can be done without creating the reference table
            var metadatas = new List<AddMetadata.MetadataItem>();

            for (int i = 0; i < 10; i++)
            {
                metadatas.Add(new AddMetadata.MetadataItem
                {
                    Key = UserMetadataType.SystemVersion.ToString(),
                    Value = _fixture.Create<string>()
                });
            }

            var command = _fixture.Build<AddMetadata.Command>()
                .With(x => x.Metadata, new AddMetadata.Metadata
                {
                    Metadatas = metadatas
                })
                .Create();

            await _mediator.Send(command);

            var actualMetadatas = _db.UserMetadatas.ToList();
            actualMetadatas.Should().NotBeEmpty();
            actualMetadatas.Count.Should().Be(metadatas.Count);
            actualMetadatas.Should().OnlyContain(actualMetadata =>
                actualMetadata.UserId == command.UserId && actualMetadata.MetadataType.ToString() == metadatas.First().Key);

            actualMetadatas.Should().OnlyContain(actualMetadata =>
                metadatas.Any(metadata => metadata.Value == actualMetadata.Value));
        }
    }
}

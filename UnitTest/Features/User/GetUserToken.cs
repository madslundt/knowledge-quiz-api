using System;
using System.Linq;
using System.Threading.Tasks;
using API.Features.User;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using UnitTest.Common;
using Xunit;

namespace UnitTest.Features.User
{
    public class GetUserByUniqueIdTest : TestBase
    {
        [Fact]
        public async Task ThrowArgumentNullExceptionWhenQueryIsNull()
        {
            var query = (GetUserToken.Query) null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUniqueIdIsNull()
        {
            var query = _fixture.Build<GetUserToken.Query>()
                .WithAutoProperties()
                .Without(x => x.UniqueId)
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUniqueIdIsEmpty()
        {
            var query = _fixture.Build<GetUserToken.Query>()
                .WithAutoProperties()
                .With(x => x.UniqueId, string.Empty)
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowArgumentNullExceptionWhenUniqueIdDoesNotExist()
        {
            var query = _fixture.Build<GetUserToken.Query>()
                .WithAutoProperties()
                .Create();

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task GetTokenForUserId()
        {
            var users = Enumerable.Range(0, 100)
                .Select(x => _fixture.Build<DataModel.Models.User.User>()
                    .WithAutoProperties()
                    .Without(xx => xx.UserQuestions)
                    .Without(xx => xx.QuestionReports)
                    .Without(xx => xx.UserAnswers)
                    .Without(xx => xx.Metadatas)
                    .Create())
                .ToList();

            _db.AddRange(users);
            _db.SaveChanges();

            var expectedUser = users[40];

            var query = _fixture.Build<GetUserToken.Query>()
                .WithAutoProperties()
                .With(x => x.UniqueId, expectedUser.UniqueId)
                .Create();

            var result = await _mediator.Send(query);

            result.Should().NotBeNull();
            result.Token.Should().NotBeEmpty();
        }
    }
}

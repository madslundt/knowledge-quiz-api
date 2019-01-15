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
                .With(x => x.TimeSpan, new TimeSpan(2, 0, 0))
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUniqueIdIsEmpty()
        {
            var query = _fixture.Build<GetUserToken.Query>()
                .WithAutoProperties()
                .With(x => x.UniqueId, string.Empty)
                .With(x => x.TimeSpan, new TimeSpan(2, 0, 0))
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenTimeSpanIsLessThan1Hour()
        {
            var query = _fixture.Build<GetUserToken.Query>()
                .WithAutoProperties()
                .With(x => x.UniqueId, string.Empty)
                .With(x => x.TimeSpan, new TimeSpan(0, 59, 59))
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenTimeSpanIsGreathanThan7Days()
        {
            var query = _fixture.Build<GetUserToken.Query>()
                .WithAutoProperties()
                .With(x => x.UniqueId, string.Empty)
                .With(x => x.TimeSpan, new TimeSpan(7, 0, 0, 0, 1))
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowArgumentNullExceptionWhenUniqueIdDoesNotExist()
        {
            var query = _fixture.Build<GetUserToken.Query>()
                .WithAutoProperties()
                .With(x => x.TimeSpan, new TimeSpan(2, 0, 0))
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
                .With(x => x.TimeSpan, new TimeSpan(2, 0, 0))
                .Create();

            var expectedExpiration = DateTime.UtcNow.Add(query.TimeSpan);

            var result = await _mediator.Send(query);

            result.Should().NotBeNull();
            result.Token.Should().NotBeEmpty();
            result.Expiration.Should().BeOnOrAfter(expectedExpiration.Subtract(new TimeSpan(0, 0, 10)));
            result.Expiration.Should().BeOnOrBefore(expectedExpiration.Add(new TimeSpan(0, 0, 10)));
        }
    }
}

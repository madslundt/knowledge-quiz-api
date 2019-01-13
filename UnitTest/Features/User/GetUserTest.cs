using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Features.User;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using UnitTest.Common;
using Xunit;

namespace UnitTest.Features.User
{
    public class GetUserTest : TestBase
    {
        [Fact]
        public async Task ThrowArgumentNullExceptionWhenQueryIsNull()
        {
            var query = (GetUser.Query) null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsNull()
        {
            var query = _fixture.Build<GetUser.Query>()
                .WithAutoProperties()
                .Without(x => x.UserId)
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsEmpty()
        {
            var query = _fixture.Build<GetUser.Query>()
                .WithAutoProperties()
                .With(x => x.UserId, Guid.Empty)
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowArgumentNullExceptionWhenUserIdDoesNotExist()
        {
            var query = _fixture.Build<GetUser.Query>()
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

            var query = _fixture.Build<GetUser.Query>()
                .WithAutoProperties()
                .With(x => x.UserId, users[40].Id)
                .Create();

            var result = await _mediator.Send(query);

            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Id.Should().Be(query.UserId);
        }
    }
}

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
    public class AddUserTest : TestBase
    {
        [Fact]
        public async Task ThrowArgumentNullExceptionWhenCommandIsNull()
        {
            var command = (AddUser.Command)null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUniqueIdIsEmpty()
        {
            var command = _fixture.Build<AddUser.Command>()
                .With(x => x.UniqueId, string.Empty)
                .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task AddUserThatDoesNotExist()
        {
            var command = _fixture.Build<AddUser.Command>()
                .Create();

            await _mediator.Send(command);

            var actualUser = _db.Users.First();
            actualUser.UniqueId.Should().Be(command.UniqueId);
        }

        [Fact]
        public async Task AddUserThatDoesExist()
        {
            var uniqueId = _fixture.Create<string>();

            var user = _fixture.Build<DataModel.Models.User.User>()
                .Without(x => x.UserAnswers)
                .Without(x => x.Metadatas)
                .Without(x => x.UserQuestions)
                .Without(x => x.QuestionReports)
                .With(x => x.UniqueId, uniqueId)
                .Create();

            _db.Users.Add(user);
            _db.SaveChanges();

            var command = _fixture.Build<AddUser.Command>()
                .With(x => x.UniqueId, uniqueId)
                .Create();

            await _mediator.Send(command);

            var actualUsers = _db.Users.ToList();
            actualUsers.Count.Should().Be(1);
            actualUsers.First().UniqueId.Should().Be(command.UniqueId);
        }
    }
}

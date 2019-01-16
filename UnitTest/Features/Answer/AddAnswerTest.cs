using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.Common;
using Xunit;

namespace UnitTest.Features.Answer
{
    public class AddAnswerTest : TestBase
    {
        [Fact]
        public async Task ThrowArgumentNullExceptionWhenCommandIsNull()
        {
            var command = (API.Features.Answer.AddAnswer.Command) null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenAnswerIdIsNull()
        {
            var command = _fixture.Build<API.Features.Answer.AddAnswer.Command>()
                            .Without(x => x.AnswerId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenAnswerIdIsEmpty()
        {
            var command = _fixture.Build<API.Features.Answer.AddAnswer.Command>()
                            .With(x => x.AnswerId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsNull()
        {
            var command = _fixture.Build<API.Features.Answer.AddAnswer.Command>()
                            .Without(x => x.UserId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsEmpty()
        {
            var command = _fixture.Build<API.Features.Answer.AddAnswer.Command>()
                            .With(x => x.UserId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowArgumentNullExceptionWhenThereAreNoAnswers()
        {
            var command = _fixture.Build<API.Features.Answer.AddAnswer.Command>()
                            .Create();

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowArgumentNullExceptionWhenAnswerIdDoesNotExist()
        {
            var answers = Enumerable.Range(0, 10)
                .Select(x => _fixture.Build<DataModel.Models.Answer.Answer>()
                            .Without(xx => xx.UserAnswers)
                            .Without(xx => xx.Question)
                            .Without(xx => xx.AnswerLocalizations)
                            .Create())
                .ToList();

            _db.AddRange(answers);
            _db.SaveChanges();

            var command = _fixture.Build<API.Features.Answer.AddAnswer.Command>()
                            .Create();

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task AddUserAnswerWhenAnswerIdExists()
        {
            var answers = Enumerable.Range(0, 10)
                .Select(x => _fixture.Build<DataModel.Models.Answer.Answer>()
                            .Without(xx => xx.UserAnswers)
                            .Without(xx => xx.Question)
                            .Without(xx => xx.AnswerLocalizations)
                            .Create())
                .ToList();

            _db.AddRange(answers);
            _db.SaveChanges();

            var expectedAnswer = answers.First();

            var command = _fixture.Build<API.Features.Answer.AddAnswer.Command>()
                            .With(x => x.AnswerId, expectedAnswer.Id)
                            .Create();

            await _mediator.Send(command);

            var actualAnswer = _db.UserAnswers.FirstOrDefault(ua => ua.AnswerId == expectedAnswer.Id);

            actualAnswer.Should().NotBeNull();
        }
    }
}

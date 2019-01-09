using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using UnitTest.Common;
using Xunit;

namespace UnitTest.Features.Question
{
    public class ReportQuestionTest : TestBase
    {
        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsNull()
        {
            var command = _fixture.Build<API.Features.Question.ReportQuestion.Command>()
                            .WithAutoProperties()
                            .Without(x => x.UserId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }
        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsEmpty()
        {
            var command = _fixture.Build<API.Features.Question.ReportQuestion.Command>()
                            .WithAutoProperties()
                            .With(x => x.UserId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }
        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionIdIsNull()
        {
            var command = _fixture.Build<API.Features.Question.ReportQuestion.Command>()
                            .WithAutoProperties()
                            .Without(x => x.QuestionId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionIdIsEmpty()
        {
            var command = _fixture.Build<API.Features.Question.ReportQuestion.Command>()
                            .WithAutoProperties()
                            .With(x => x.QuestionId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowArgumentNullExceptionWhenCommandIsNull()
        {
            var command = (API.Features.Question.ReportQuestion.Command) null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ReportQuestionId()
        {
            var command = _fixture.Build<API.Features.Question.ReportQuestion.Command>()
                            .WithAutoProperties()
                            .Create();

            await _mediator.Send(command);

            var report = _db.QuestionReports.First();
            report.Should().NotBeNull();
            report.QuestionId.Should().Be(command.QuestionId);
            report.UserId.Should().Be(command.UserId);
        }
    }
}

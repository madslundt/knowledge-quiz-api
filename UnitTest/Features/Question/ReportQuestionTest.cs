using System;
using System.Collections.Generic;
using System.Text;
using UnitTest.Common;

namespace UnitTest.Features.Question
{
    public class ReportQuestionTest : TestBase
    {
        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsNull()
        {
            var query = _fixture.Build<API.Features.Question.ReportQuestion.Command>()
                            .WithAutoProperties()
                            .Without(x => x.UserId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }
        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsEmpty()
        {
            var query = _fixture.Build<API.Features.Question.ReportQuestion.Command>()
                            .WithAutoProperties()
                            .With(x => x.UserId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }
        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionIdIsNull()
        {
            var query = _fixture.Build<API.Features.Question.ReportQuestion.Command>()
                            .WithAutoProperties()
                            .Without(x => x.QuestionId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionIdIsEmpty()
        {
            var query = _fixture.Build<API.Features.Question.ReportQuestion.Command>()
                            .WithAutoProperties()
                            .With(x => x.QuestionId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenCommandIsNull()
        {
            var query = (API.Features.Question.ReportQuestion.Command) null;

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }
    }
}

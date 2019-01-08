﻿using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Common;
using Xunit;

namespace UnitTest.Features.Question
{
    public class MarkUserQuestionTest : TestBase
    {
        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsNull()
        {
            var command = _fixture.Build<API.Features.Question.MarkUserQuestion.Command>()
                            .WithAutoProperties()
                            .Without(x => x.UserId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsEmpty()
        {
            var command = _fixture.Build<API.Features.Question.MarkUserQuestion.Command>()
                            .WithAutoProperties()
                            .With(x => x.UserId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionsAreNull()
        {
            var command = _fixture.Build<API.Features.Question.MarkUserQuestion.Command>()
                            .WithAutoProperties()
                            .Without(x => x.Questions)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionsAreEmpty()
        {
            var command = _fixture.Build<API.Features.Question.MarkUserQuestion.Command>()
                            .WithAutoProperties()
                            .With(x => x.Questions, new List<API.Features.Question.MarkUserQuestion.Question>())
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionsContainEmptyId()
        {
            var command = _fixture.Build<API.Features.Question.MarkUserQuestion.Command>()
                            .WithAutoProperties()
                            .With(x => x.Questions, new List<API.Features.Question.MarkUserQuestion.Question>
                            {
                                new API.Features.Question.MarkUserQuestion.Question
                                {
                                    QuestionId = Guid.Empty
                                }
                            })
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
        }

        [Fact]
        public async Task AddUserQuestionWithHintUsedFalse()
        {
            var markQuestions = Enumerable.Range(0, 10)
                .Select(x => _fixture.Build<API.Features.Question.MarkUserQuestion.Question>()
                                .With(xx => xx.HintUsed, false)
                                .Create())
                .ToList();

            var command = _fixture.Build<API.Features.Question.MarkUserQuestion.Command>()
                            .WithAutoProperties()
                            .With(x => x.Questions, markQuestions)
                            .Create();

            await _mediator.Send(command);

            var userQuestions = _db.UserQuestions.ToList();

            userQuestions.Where(uq => uq.UserId == command.UserId).Count().Should().Be(10);
            userQuestions.Where(uq => uq.HintUsed).Should().BeEmpty();
            var qIds = markQuestions.Select(mq => mq.QuestionId);
            userQuestions.Should().Contain(uq => qIds.Any(qid => qid == uq.QuestionId));
        }

        [Fact]
        public async Task AddUserQuestionWithHintUsedTrue()
        {
            var markQuestions = Enumerable.Range(0, 10)
                .Select(x => _fixture.Build<API.Features.Question.MarkUserQuestion.Question>()
                                .With(xx => xx.HintUsed, true)
                                .Create())
                .ToList();

            var command = _fixture.Build<API.Features.Question.MarkUserQuestion.Command>()
                            .WithAutoProperties()
                            .With(x => x.Questions, markQuestions)
                            .Create();

            await _mediator.Send(command);

            var userQuestions = _db.UserQuestions.ToList();

            userQuestions.Where(uq => uq.UserId == command.UserId).Count().Should().Be(10);
            userQuestions.Where(uq => !uq.HintUsed).Should().BeEmpty();
            var qIds = markQuestions.Select(mq => mq.QuestionId);
            userQuestions.Should().Contain(uq => qIds.Any(qid => qid == uq.QuestionId));
        }

        [Fact]
        public async Task AddUserQuestion()
        {
            var markQuestions = Enumerable.Range(0, 10)
                .Select(x => _fixture.Build<API.Features.Question.MarkUserQuestion.Question>()
                                .Create())
                .ToList();

            var command = _fixture.Build<API.Features.Question.MarkUserQuestion.Command>()
                            .WithAutoProperties()
                            .With(x => x.Questions, markQuestions)
                            .Create();

            await _mediator.Send(command);

            var userQuestions = _db.UserQuestions.ToList();

            userQuestions.Where(uq => uq.UserId == command.UserId).Count().Should().Be(10);
            userQuestions.Where(uq => !uq.HintUsed).Count().Should().Be(markQuestions.Where(mq => !mq.HintUsed).Count());
            userQuestions.Where(uq => uq.HintUsed).Count().Should().Be(markQuestions.Where(mq => mq.HintUsed).Count());
            var qIds = markQuestions.Select(mq => mq.QuestionId);
            userQuestions.Should().Contain(uq => qIds.Any(qid => qid == uq.QuestionId));
        }
    }
}

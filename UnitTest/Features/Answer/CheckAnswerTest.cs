﻿using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.Common;
using Xunit;

namespace UnitTest.Features.Answer
{
    public class CheckAnswerTest : TestBase
    {
        [Fact]
        public async Task ThrowArgumentNullExceptionWhenQueryIsNull()
        {
            var query = (API.Features.Answer.CheckAnswer.Query) null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenAnswerIsNull()
        {
            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .Without(x => x.Answer)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenAnswerIdIsNull()
        {
            var answer = _fixture.Build<API.Features.Answer.CheckAnswer.AnswerRequest>()
                            .Without(x => x.AnswerId)
                            .Create();

            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .With(x => x.Answer, answer)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenAnswerIdIsEmpty()
        {
            var answer = _fixture.Build<API.Features.Answer.CheckAnswer.AnswerRequest>()
                            .With(x => x.AnswerId, Guid.Empty)
                            .Create();

            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .With(x => x.Answer, answer)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionIdIsNull()
        {
            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .Without(x => x.QuestionId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionIdIsEmpty()
        {
            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .With(x => x.QuestionId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenLocaleIsEmpty()
        {
            var answer = _fixture.Build<API.Features.Answer.CheckAnswer.AnswerRequest>()
                            .Create();

            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .Without(x => x.Locale)
                            .With(x => x.Answer, answer)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenAnswerIdDoesNotExist()
        {
            var answer = _fixture.Build<API.Features.Answer.CheckAnswer.AnswerRequest>()
                            .Create();

            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .With(x => x.Answer, answer)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenAnswerIdDoesNotBelongToAQuestion()
        {
            var answer = _fixture.Build<DataModel.Models.Answer.Answer>()
                            .Without(x => x.Question)
                            .Without(x => x.QuestionId)
                            .Without(x => x.UserAnswers)
                            .Without(x => x.AnswerLocalizations)
                            .Create();

            _db.Answers.Add(answer);
            _db.SaveChanges();

            var answerRequest = _fixture.Build<API.Features.Answer.CheckAnswer.AnswerRequest>()
                            .With(x => x.AnswerId, answer.Id)
                            .Create();

            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .With(x => x.Answer, answerRequest)
                            .With(x => x.QuestionId, answer.QuestionId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task GetCorrectAnswer()
        {
            var question = _fixture.Build<DataModel.Models.Question.Question>()
                            .Without(x => x.UserQuestions)
                            .Without(x => x.Answers)
                            .Without(x => x.QuestionLocalizations)
                            .Without(x => x.QuestionReports)
                            .Create();

            var answers = Enumerable.Range(0, 10)
                .Select(_ => _fixture.Build<DataModel.Models.Answer.Answer>()
                            .With(x => x.QuestionId, question.Id)
                            .With(x => x.IsCorrect, false)
                            .Without(x => x.Question)
                            .Without(x => x.UserAnswers)
                            .Without(x => x.AnswerLocalizations)
                            .Create())
                .ToList();

            answers.First().IsCorrect = true;

            _db.Questions.Add(question);
            _db.Answers.AddRange(answers);
            _db.SaveChanges();

            var expectedAnswer = answers.First();

            var answerRequest = _fixture.Build<API.Features.Answer.CheckAnswer.AnswerRequest>()
                            .With(x => x.AnswerId, expectedAnswer.Id)
                            .Create();

            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .With(x => x.Answer, answerRequest)
                            .With(x => x.QuestionId, expectedAnswer.QuestionId)
                            .Create();

            var result = await _mediator.Send(query);

            result.Should().NotBeNull();
            result.IsCorrect.Should().BeTrue();
            result.CorrectAnswers.Count.Should().Be(1);
            result.CorrectAnswers.First().Id.Should().Be(expectedAnswer.Id);
        }

        [Fact]
        public async Task GetAnotherCorrectAnswer()
        {
            var question = _fixture.Build<DataModel.Models.Question.Question>()
                            .Without(x => x.UserQuestions)
                            .Without(x => x.Answers)
                            .Without(x => x.QuestionLocalizations)
                            .Without(x => x.QuestionReports)
                            .Create();

            var answers = Enumerable.Range(0, 10)
                .Select(_ => _fixture.Build<DataModel.Models.Answer.Answer>()
                            .With(x => x.QuestionId, question.Id)
                            .With(x => x.IsCorrect, true)
                            .Without(x => x.Question)
                            .Without(x => x.UserAnswers)
                            .Without(x => x.AnswerLocalizations)
                            .Create())
                .ToList();

            answers.First().IsCorrect = false;

            _db.Questions.Add(question);
            _db.Answers.AddRange(answers);
            _db.SaveChanges();

            var expectedAnswer = answers.First();

            var answerRequest = _fixture.Build<API.Features.Answer.CheckAnswer.AnswerRequest>()
                            .With(x => x.AnswerId, expectedAnswer.Id)
                            .Create();

            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .With(x => x.Answer, answerRequest)
                            .With(x => x.QuestionId, expectedAnswer.QuestionId)
                            .Create();

            var result = await _mediator.Send(query);

            result.Should().NotBeNull();
            result.IsCorrect.Should().BeFalse();
            result.CorrectAnswers.Any(answer => answer.Id == expectedAnswer.Id).Should().BeFalse();
            result.CorrectAnswers.Count.Should().Be(answers.Count - 1);
        }
    }
}

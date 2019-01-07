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
    public class CheckAnswerTest : TestBase
    {
        [Fact]
        public async Task ThrowValidationExceptionWhenAnswerIdIsNull()
        {
            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .WithAutoProperties()
                            .Without(x => x.AnswerId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenAnswerIdIsEmpty()
        {
            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .WithAutoProperties()
                            .With(x => x.AnswerId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenLocaleIsEmpty()
        {
            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .WithAutoProperties()
                            .Without(x => x.Locale)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowArgumentNullExceptionWhenAnswerIdDoesNotExist()
        {
            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .WithAutoProperties()
                            .Create();

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowArgumentNullExceptionWhenAnswerIdDoesNotBelongToAQuestion()
        {
            var answer = _fixture.Build<DataModel.Models.Answer.Answer>()
                            .WithAutoProperties()
                            .Without(x => x.Question)
                            .Without(x => x.QuestionId)
                            .Without(x => x.UserAnswers)
                            .Without(x => x.AnswerLocalizations)
                            .Create();

            _db.Answers.Add(answer);
            _db.SaveChanges();

            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .With(x => x.AnswerId, answer.Id)
                            .Create();

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task GetCorrectAnswer()
        {
            var question = _fixture.Build<DataModel.Models.Question.Question>()
                            .WithAutoProperties()
                            .Without(x => x.UserQuestions)
                            .Without(x => x.Answers)
                            .Without(x => x.QuestionLocalizations)
                            .Create();

            var answers = Enumerable.Range(0, 10)
                .Select(x => _fixture.Build<DataModel.Models.Answer.Answer>()
                            .WithAutoProperties()
                            .With(xx => xx.QuestionId, question.Id)
                            .With(xx => xx.IsCorrect, false)
                            .Without(xx => xx.Question)
                            .Without(xx => xx.UserAnswers)
                            .Without(xx => xx.AnswerLocalizations)
                            .Create())
                .ToList();

            answers.First().IsCorrect = true;

            _db.Questions.Add(question);
            _db.Answers.AddRange(answers);
            _db.SaveChanges();

            var expectedAnswer = answers.First();

            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .With(x => x.AnswerId, expectedAnswer.Id)
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
                            .WithAutoProperties()
                            .Without(x => x.UserQuestions)
                            .Without(x => x.Answers)
                            .Without(x => x.QuestionLocalizations)
                            .Create();

            var answers = Enumerable.Range(0, 10)
                .Select(x => _fixture.Build<DataModel.Models.Answer.Answer>()
                            .WithAutoProperties()
                            .With(xx => xx.QuestionId, question.Id)
                            .With(xx => xx.IsCorrect, true)
                            .Without(xx => xx.Question)
                            .Without(xx => xx.UserAnswers)
                            .Without(xx => xx.AnswerLocalizations)
                            .Create())
                .ToList();

            answers.First().IsCorrect = false;

            _db.Questions.Add(question);
            _db.Answers.AddRange(answers);
            _db.SaveChanges();

            var expectedAnswer = answers.First();

            var query = _fixture.Build<API.Features.Answer.CheckAnswer.Query>()
                            .With(x => x.AnswerId, expectedAnswer.Id)
                            .Create();

            var result = await _mediator.Send(query);

            result.Should().NotBeNull();
            result.IsCorrect.Should().BeFalse();
            result.CorrectAnswers.Any(answer => answer.Id == expectedAnswer.Id).Should().BeFalse();
            result.CorrectAnswers.Count.Should().Be(answers.Count - 1);
        }
    }
}

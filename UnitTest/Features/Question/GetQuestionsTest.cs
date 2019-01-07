using AutoFixture;
using DataModel.Models.Question;
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
    public class GetQuestionsTest : TestBase
    {
        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsNull()
        {
            var query = _fixture.Build<API.Features.Question.GetQuestions.Query>()
                            .WithAutoProperties()
                            .Without(x => x.UserId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenUserIdIsEmpty()
        {
            var query = _fixture.Build<API.Features.Question.GetQuestions.Query>()
                            .WithAutoProperties()
                            .With(x => x.UserId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenLocaleIsEmpty()
        {
            var query = _fixture.Build<API.Features.Question.GetQuestions.Query>()
                            .WithAutoProperties()
                            .Without(x => x.Locale)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenLimitIsGreaterThan50()
        {
            var query = _fixture.Build<API.Features.Question.GetQuestions.Query>()
                            .WithAutoProperties()
                            .With(x => x.Limit, 51)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenLimitIsLessThan1()
        {
            var query = _fixture.Build<API.Features.Question.GetQuestions.Query>()
                            .WithAutoProperties()
                            .With(x => x.Limit, 0)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task GetAListOfQuestions()
        {
            var questions = Enumerable.Range(0, 100)
                .Select(x => _fixture.Build<DataModel.Models.Question.Question>()
                            .WithAutoProperties()
                            .Without(xx => xx.UserQuestions)
                            .With(xx => xx.Answers, GetAnswers())
                            .Without(xx => xx.QuestionLocalizations)
                            .Create())
                .ToList();

            var localizations = GetLocalizations();

            var questionLocalizations = questions.Select(question => Enumerable.Range(0, localizations.Count)
                .Select(x => _fixture.Build<QuestionLocalization>()
                            .WithAutoProperties()
                            .With(xx => xx.QuestionId, question.Id)
                            .With(xx => xx.Question, question)
                            .Create())
                .ToList());

            _db.Questions.AddRange(questions);
            _db.SaveChanges();
        }

        private ICollection<DataModel.Models.Localization.LocaleReference> GetLocalizations()
        {
            var localizations = Enumerable.Range(0, Enum.GetValues(typeof(DataModel.Models.Localization.Locale)).Length)
                            .Select(x => _fixture.Build<DataModel.Models.Localization.LocaleReference>()
                                        .WithAutoProperties()
                                        .Create())
                            .ToList();

            return localizations;
        }

        private ICollection<DataModel.Models.Answer.Answer> GetAnswers()
        {
            var answers = Enumerable.Range(0, 4)
                .Select(x => _fixture.Build<DataModel.Models.Answer.Answer>()
                            .WithAutoProperties()
                            .Without(xx => xx.QuestionId)
                            .Without(xx => xx.Question)
                            .Without(xx => xx.UserAnswers)
                            .Without(xx => xx.AnswerLocalizations)
                            .Create())
                .ToList();

            return answers;
        }
    }
}

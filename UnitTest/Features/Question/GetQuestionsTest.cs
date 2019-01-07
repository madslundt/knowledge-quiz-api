using AutoFixture;
using DataModel.Models.Question;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.Models;
using DataModel.Models.User;
using FluentAssertions;
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

            var questionLocalizations = GetQuestionLocalizations(questions, localizations);

            var user = _fixture.Build<User>()
                .WithAutoProperties()
                .Without(x => x.UserAnswers)
                .Without(x => x.UserQuestions)
                .Without(x => x.Metadata)
                .Create();

            _db.Localizations.AddRange(localizations);
            _db.QuestionLocalizations.AddRange(questionLocalizations);
            _db.Questions.AddRange(questions);
            _db.Users.Add(user);
            _db.SaveChanges();

            var query = _fixture.Build<API.Features.Question.GetQuestions.Query>()
                .WithAutoProperties()
                .With(x => x.UserId, user.Id)
                .With(x => x.Locale, DataModel.Models.Localization.Locale.en_US)
                .Without(x => x.Limit)
                .Create();

            var result = await _mediator.Send(query);

            result.Questions.Should().NotBeEmpty();
            result.Questions.Count.Should().Be(20);
        }

        private ICollection<DataModel.Models.Question.QuestionLocalization> GetQuestionLocalizations(
            ICollection<DataModel.Models.Question.Question> questions, ICollection<DataModel.Models.Localization.Localization> localizations)
        {
            var result = new List<DataModel.Models.Question.QuestionLocalization>();

            foreach (var question in questions)
            {
                foreach (var localization in localizations)
                {
                    foreach (var type in Enum.GetValues(typeof(QuestionType)))
                    {
                        var questionLocalization = _fixture.Build<QuestionLocalization>()
                            .WithAutoProperties()
                            .With(x => x.QuestionId, question.Id)
                            .With(x => x.Question, question)
                            .With(x => x.QuestionType, type)
                            .With(x => x.LocalizationId, localization.Id)
                            .With(x => x.Localization, localization)
                            .Create();

                        result.Add(questionLocalization);
                    }
                }
            }

            return result;
        }

        private ICollection<DataModel.Models.Localization.Localization> GetLocalizations()
        {
            var localizations = Enum.GetNames(typeof(DataModel.Models.Localization.Locale))
                            .Select(x => _fixture.Build<DataModel.Models.Localization.Localization>()
                                        .WithAutoProperties()
                                        .Without(xx => xx.AnswerLocalizations)
                                        .Without(xx => xx.QuestionLocalizations)
                                        .Without(xx => xx.LocaleReference)
                    .With(xx => xx.Locale, Enum.Parse<DataModel.Models.Localization.Locale>(x))
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

using AutoFixture;
using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;
using DataModel.Models.User;
using FluentAssertions;
using UnitTest.Common;
using Xunit;

namespace UnitTest.Features.Question
{
    public class GetQuestionsTest : TestBase
    {
        [Fact]
        public async Task ThrowArgumentNullExceptionWhenQueryIsNull()
        {
            var command = (API.Features.Question.GetQuestions.Query) null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(command));
        }

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
            var questions = Enumerable.Range(0, 50)
                .Select(x => _fixture.Build<DataModel.Models.Question.Question>()
                            .WithAutoProperties()
                            .Without(xx => xx.UserQuestions)
                            .With(xx => xx.Answers, _seedData.GetAnswers())
                            .Without(xx => xx.QuestionLocalizations)
                            .Without(xx => xx.QuestionReports)
                            .Create())
                .ToList();

            var localizations = _seedData.GetLocalizations();

            var questionLocalizations = _seedData.GetQuestionLocalizations(questions, localizations.ToList());

            var user = _fixture.Build<DataModel.Models.User.User>()
                .WithAutoProperties()
                .Without(x => x.UserAnswers)
                .Without(x => x.UserQuestions)
                .Without(x => x.Metadatas)
                .Without(x => x.QuestionReports)
                .Create();

            _db.Localizations.AddRange(localizations);
            _db.QuestionLocalizations.AddRange(questionLocalizations);
            _db.Questions.AddRange(questions);
            _db.Users.Add(user);
            _db.SaveChanges();

            var answers = _db.Answers.ToList();
            var answerLocalizations = _seedData.GetAnswerLocalizations(answers, localizations.ToList());

            _db.AnswerLocalizations.AddRange(answerLocalizations);
            _db.SaveChanges();

            var query = _fixture.Build<API.Features.Question.GetQuestions.Query>()
                .WithAutoProperties()
                .With(x => x.UserId, user.Id)
                .With(x => x.Locale, DataModel.Models.Localization.Locale.en_US)
                .Without(x => x.Limit)
                .Create();

            var result = await _mediator.Send(query);

            var currentLocaleLocalizations =
                localizations.Where(localization => localization.Locale == query.Locale).ToList();

            result.Questions.Should().NotBeEmpty();
            result.Questions.Count.Should().Be(20);
            result.Questions.Should().OnlyContain(question =>
                currentLocaleLocalizations.Any(localization => localization.Text == question.Text));
        }

        [Fact]
        public async Task GetAListWithOneQuestionsWhenLimitIs1()
        {
            var questions = Enumerable.Range(0, 50)
                .Select(x => _fixture.Build<DataModel.Models.Question.Question>()
                            .WithAutoProperties()
                            .Without(xx => xx.UserQuestions)
                            .With(xx => xx.Answers, _seedData.GetAnswers())
                            .Without(xx => xx.QuestionLocalizations)
                            .Without(xx => xx.QuestionReports)
                            .Create())
                .ToList();

            var localizations = _seedData.GetLocalizations();

            var questionLocalizations = _seedData.GetQuestionLocalizations(questions, localizations.ToList());

            var user = _fixture.Build<DataModel.Models.User.User>()
                .WithAutoProperties()
                .Without(x => x.UserAnswers)
                .Without(x => x.UserQuestions)
                .Without(x => x.Metadatas)
                .Without(x => x.QuestionReports)
                .Create();

            _db.Localizations.AddRange(localizations);
            _db.QuestionLocalizations.AddRange(questionLocalizations);
            _db.Questions.AddRange(questions);
            _db.Users.Add(user);
            _db.SaveChanges();

            var answers = _db.Answers.ToList();
            var answerLocalizations = _seedData.GetAnswerLocalizations(answers, localizations.ToList());

            _db.AnswerLocalizations.AddRange(answerLocalizations);
            _db.SaveChanges();

            var query = _fixture.Build<API.Features.Question.GetQuestions.Query>()
                .WithAutoProperties()
                .With(x => x.UserId, user.Id)
                .With(x => x.Locale, DataModel.Models.Localization.Locale.en_US)
                .With(x => x.Limit, 1)
                .Create();

            var result = await _mediator.Send(query);

            var currentLocaleLocalizations =
                localizations.Where(localization => localization.Locale == query.Locale).ToList();

            result.Questions.Should().NotBeEmpty();
            result.Questions.Count.Should().Be(1);
            result.Questions.Should().OnlyContain(question =>
                currentLocaleLocalizations.Any(localization => localization.Text == question.Text));
        }
    }
}

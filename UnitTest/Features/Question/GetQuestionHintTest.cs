using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.Common;
using Xunit;

namespace UnitTest.Features.Question
{
    public class GetQuestionHintTest : TestBase
    {
        [Fact]
        public async Task ThrowArgumentNullExceptionWhenQueryIsNull()
        {
            var query = (API.Features.Question.GetQuestionHint.Query) null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenLocaleIsEmpty()
        {
            var query = _fixture.Build<API.Features.Question.GetQuestionHint.Query>()
                            .WithAutoProperties()
                            .Without(x => x.Locale)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionIdIsEmpty()
        {
            var query = _fixture.Build<API.Features.Question.GetQuestionHint.Query>()
                            .WithAutoProperties()
                            .With(x => x.QuestionId, Guid.Empty)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowValidationExceptionWhenQuestionIdIsNull()
        {
            var query = _fixture.Build<API.Features.Question.GetQuestionHint.Query>()
                            .WithAutoProperties()
                            .Without(x => x.QuestionId)
                            .Create();

            await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task ThrowArgumentNullExceptionWhenQuestionIdDoesNotExist()
        {
            var query = _fixture.Build<API.Features.Question.GetQuestionHint.Query>()
                            .WithAutoProperties()
                            .Create();

            await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send(query));
        }

        [Fact]
        public async Task GetHintForQuestionId()
        {
            var questions = _seedData.GetQuestions();

            var localizations = _seedData.GetLocalizations();

            var questionLocalizations = _seedData.GetQuestionLocalizations(questions, localizations);

            _db.Questions.AddRange(questions);
            _db.Localizations.AddRange(localizations);
            _db.QuestionLocalizations.AddRange(questionLocalizations);
            _db.SaveChanges();

            var locale = localizations.First().Locale;
            var expectedQuestionLocalization = _db.QuestionLocalizations.Include(ql => ql.Localization).First(ql => ql.QuestionId == questions.First().Id && ql.Localization.Locale == locale);

            var query = _fixture.Build<API.Features.Question.GetQuestionHint.Query>()
                            .WithAutoProperties()
                            .With(x => x.QuestionId, expectedQuestionLocalization.QuestionId)
                            .With(x => x.Locale, locale)
                            .Create();

            var result = await _mediator.Send(query);

            result.Should().NotBeNull();
            result.Text.Should().Be(expectedQuestionLocalization.Localization.Text);
        }
    }
}

using AutoFixture;
using DataModel.Models;
using DataModel.Models.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using DataModel.Models.Answer;
using DataModel.Models.Localization;

namespace UnitTest.Common
{
    public class SeedData
    {
        private readonly Fixture _fixture;
        public SeedData(Fixture fixture)
        {
            _fixture = fixture;
        }

        public ICollection<QuestionLocalization> GetQuestionLocalizations(
            ICollection<Question> questions, IList<Localization> localizations)
        {
            var result = new List<QuestionLocalization>();

            var random = new Random();
            foreach (var question in questions)
            {
                foreach (var type in Enum.GetValues(typeof(QuestionType)))
                {
                    int index = random.Next(localizations.Count);
                    var localization = localizations[index];

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

            return result;
        }

        public ICollection<Localization> GetLocalizations(int limit = 100)
        {
            var localizations = new List<Localization>();
            foreach (var l in Enum.GetNames(typeof(Locale)))
            {
                localizations.AddRange(Enumerable.Range(0, limit)
                    .Select(xx => _fixture.Build<Localization>()
                        .WithAutoProperties()
                        .Without(xxx => xxx.AnswerLocalizations)
                        .Without(xxx => xxx.QuestionLocalizations)
                        .Without(xxx => xxx.LocaleReference)
                        .With(xxx => xxx.Locale, Enum.Parse<Locale>(l))
                        .Create())
                    .OrderBy(xx => Guid.NewGuid()).ToList());
            }

            return localizations;
        }

        public ICollection<Answer> GetAnswers(int limit = 4)
        {
            var answers = Enumerable.Range(0, limit)
                .Select(x => _fixture.Build<Answer>()
                            .WithAutoProperties()
                            .Without(xx => xx.QuestionId)
                            .Without(xx => xx.Question)
                            .Without(xx => xx.UserAnswers)
                            .Without(xx => xx.AnswerLocalizations)
                            .Create())
                .ToList();

            return answers;
        }

        public ICollection<Question> GetQuestions(int limit = 100)
        {
            var questions = Enumerable.Range(0, limit)
                .Select(x => _fixture.Build<Question>()
                            .WithAutoProperties()
                            .Without(xx => xx.UserQuestions)
                            .Without(xx => xx.QuestionLocalizations)
                            .Without(xx => xx.QuestionReports)
                            .Without(xx => xx.Answers)
                            .Create())
                .ToList();

            return questions;
        }

        public ICollection<AnswerLocalization> GetAnswerLocalizations(ICollection<Answer> answers, IList<Localization> localizations)
        {
            var result = new List<AnswerLocalization>();

            var random = new Random();

            foreach (var answer in answers)
            {
                int index = random.Next(localizations.Count);
                var localization = localizations[index];

                var answerLocalization = _fixture.Build<AnswerLocalization>()
                        .WithAutoProperties()
                        .With(x => x.AnswerId, answer.Id)
                        .With(x => x.Answer, answer)
                        .With(x => x.LocalizationId, localization.Id)
                        .With(x => x.Localization, localization)
                        .Create();

                result.Add(answerLocalization);
            }

            return result;
        }
    }
}

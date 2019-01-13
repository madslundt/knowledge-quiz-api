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

            var localizationsGroupByLanguage =
                localizations.GroupBy(l => l.Locale).ToDictionary(k => k.Key, v => v.ToList());

            var random = new Random();
            foreach (var question in questions)
            {
                foreach (var type in Enum.GetValues(typeof(QuestionType)))
                {
                    foreach (KeyValuePair<Locale, List<Localization>> entry in localizationsGroupByLanguage)
                    {
                        int index = random.Next(entry.Value.Count);
                        var localization = entry.Value[index];

                        var questionLocalization = _fixture.Build<QuestionLocalization>()
                            .WithAutoProperties()
                            .Without(x => x.QuestionTypeReference)
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

        public ICollection<Localization> GetLocalizations(int limit = 100)
        {
            var localizations = new List<Localization>();

            foreach (var l in Enum.GetNames(typeof(Locale)))
            {
                localizations.AddRange(Enumerable.Range(0, limit)
                    .Select(_ => _fixture.Build<Localization>()
                        .WithAutoProperties()
                        .Without(x => x.AnswerLocalizations)
                        .Without(x => x.QuestionLocalizations)
                        .Without(x => x.LocaleReference)
                        .With(x => x.Locale, Enum.Parse<Locale>(l))
                        .Create())
                    .OrderBy(_ => Guid.NewGuid()).ToList());
            }

            return localizations;
        }

        public ICollection<Answer> GetAnswers(int limit = 4)
        {
            var answers = Enumerable.Range(0, limit)
                .Select(_ => _fixture.Build<Answer>()
                            .WithAutoProperties()
                            .Without(x => x.QuestionId)
                            .Without(x => x.Question)
                            .Without(x => x.UserAnswers)
                            .Without(x => x.AnswerLocalizations)
                            .Create())
                .ToList();

            return answers;
        }

        public ICollection<Question> GetQuestions(int limit = 100)
        {
            var questions = Enumerable.Range(0, limit)
                .Select(_ => _fixture.Build<Question>()
                            .WithAutoProperties()
                            .Without(x => x.UserQuestions)
                            .Without(x => x.QuestionLocalizations)
                            .Without(x => x.QuestionReports)
                            .Without(x => x.Answers)
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

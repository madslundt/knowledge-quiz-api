﻿using AutoFixture;
using DataModel.Models;
using DataModel.Models.Question;
using System;
using System.Collections.Generic;
using System.Linq;

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
            ICollection<Question> questions, ICollection<DataModel.Models.Localization.Localization> localizations)
        {
            var result = new List<QuestionLocalization>();

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

        public ICollection<DataModel.Models.Localization.Localization> GetLocalizations()
        {
            var localizations = Enum.GetNames(typeof(DataModel.Models.Localization.Locale))
                            .Select(x => _fixture.Build<DataModel.Models.Localization.Localization>()
                                        .WithAutoProperties()
                                        .Without(xx => xx.AnswerLocalizations)
                                        .Without(xx => xx.QuestionLocalizations)
                                        .Without(xx => xx.LocaleReference)
                    .With(xx => xx.Locale, Enum.Parse<DataModel.Models.Localization.Locale>(x))
                                        .Create())
                            .OrderBy(x => Guid.NewGuid()).ToList();

            return localizations;
        }

        public ICollection<DataModel.Models.Answer.Answer> GetAnswers(int limit = 4)
        {
            var answers = Enumerable.Range(0, limit)
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
    }
}
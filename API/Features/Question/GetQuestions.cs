using DataModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Question
{
    public class GetQuestions
    {
        public class Query : IRequest<Result>
        {
            public Guid UserId { get; set; }
            public DataModel.Models.Localization.Locale Locale { get; set; }
            public int Limit { get; set; } = 20;
        }

        public class Result
        {
            public ICollection<Question> Questions { get; set; }
        }

        public class Question
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
            public string ImageUrl { get; set; }
            public ICollection<Answer> Answers { get; set; }
        }

        public class Answer
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
        }

        public class GetQuestionsValidator : AbstractValidator<Query>
        {
            public GetQuestionsValidator()
            {
                RuleFor(query => query.UserId).NotEmpty();
                RuleFor(query => query.Locale).IsInEnum();
                RuleFor(query => query.Limit).InclusiveBetween(1, 50)
                    .WithMessage($"{nameof(Query.Limit)} must be between 1 and 50");
            }
        }


        public class GetQuestionsHandler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;

            public GetQuestionsHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var answeredQuestionIds = await GetAnsweredQuestionIds(message.UserId);
                var questions = await GetQuestions(answeredQuestionIds, message.Locale, message.Limit);

                var result = new Result
                {
                    Questions = questions
                };

                return result;
            }

            private async Task<ICollection<Guid>> GetAnsweredQuestionIds(Guid userId)
            {
                var query = from userAnswer in _db.UserAnswers
                    join answer in _db.Answers on userAnswer.AnswerId equals answer.Id
                    where userAnswer.UserId == userId
                    select answer.QuestionId;

                var result = await query.ToListAsync();

                return result;
            }

            private async Task<ICollection<Question>> GetQuestions(ICollection<Guid> skipQuestionIds,
                DataModel.Models.Localization.Locale locale, int limit)
            {
                var query = from question in _db.Questions
                    join answer in _db.Answers on question.Id equals answer.QuestionId
                    join questionLocalization in _db.QuestionLocalizations on question.Id equals questionLocalization
                        .QuestionId
                    join answerLocalization in _db.AnswerLocalizations on answer.Id equals answerLocalization.AnswerId
                    join qLocalization in _db.Localizations on questionLocalization.LocalizationId equals qLocalization
                        .Id
                    join aLocalization in _db.Localizations on answerLocalization.LocalizationId equals aLocalization.Id
                    where !skipQuestionIds.Contains(question.Id) && qLocalization.Locale == locale &&
                          aLocalization.Locale == locale && questionLocalization.QuestionType == QuestionType.Question
                    orderby Guid.NewGuid()
                    group new {answer, aLocalization} by new {question, qLocalization}
                    into gr
                    select new Question
                    {
                        Id = gr.Key.question.Id,
                        Text = gr.Key.qLocalization.Text,
                        ImageUrl = gr.Key.question.ImageUrl,

                        Answers = gr.Select(a => new Answer
                        {
                            Id = a.answer.Id,
                            Text = a.aLocalization.Text
                        }).ToList()
                    };

                var result = await query.Take(limit).ToListAsync();

                return result;
            }
        }
    }
}

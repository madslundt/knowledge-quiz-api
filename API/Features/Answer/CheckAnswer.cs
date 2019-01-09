using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Answer
{
    public class CheckAnswer
    {
        public class Query : IRequest<Result>
        {
            public Guid AnswerId { get; set; }
            public DataModel.Models.Localization.Locale Locale { get; set; }
        }

        public class Result
        {
            public bool IsCorrect { get; set; }
            public ICollection<Answer> CorrectAnswers { get; set; }
            public string Text { get; set; }
        }

        public class Answer
        {
            public Guid Id { get; set; }
        }

        public class CheckAnswerValidator : AbstractValidator<Query>
        {
            public CheckAnswerValidator()
            {
                RuleFor(query => query).NotNull();
                RuleFor(query => query.AnswerId).NotEmpty();
                RuleFor(query => query.Locale).IsInEnum();
            }
        }


        public class CheckAnswerHandler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;

            public CheckAnswerHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var correctAnswers = await GetCorrectAnswers(message.AnswerId);

                if (correctAnswers.Count == 0)
                {
                    throw new ValidationException($"No correct answers for {nameof(message.AnswerId)} '{message.AnswerId}'");
                }

                var text = await GetQuestionText(message.AnswerId, message.Locale);

                var result = new Result
                {
                    IsCorrect = correctAnswers.Any(answer => answer.Id == message.AnswerId),
                    CorrectAnswers = correctAnswers,
                    Text = text
                };

                return result;
            }

            private async Task<string> GetQuestionText(Guid answerId, DataModel.Models.Localization.Locale locale)
            {
                var query = from answer in _db.Answers
                            join questionLocalization in _db.QuestionLocalizations on answer.QuestionId equals questionLocalization.QuestionId
                            join localization in _db.Localizations on questionLocalization.LocalizationId equals localization.Id
                            where answer.Id == answerId && questionLocalization.QuestionType == DataModel.Models.QuestionType.Final
                            where localization.Locale == locale
                            select localization.Text;

                var result = await query.FirstOrDefaultAsync();

                return result;
            }

            private async Task<ICollection<Answer>> GetCorrectAnswers(Guid answerId)
            {
                var query = from answer in _db.Answers
                            join question in _db.Questions on answer.QuestionId equals question.Id
                            join answer2 in _db.Answers on question.Id equals answer2.QuestionId
                            where answer.Id == answerId && answer2.IsCorrect
                            select new Answer
                            {
                                Id = answer2.Id
                            };

                var result = await query.ToListAsync();

                return result;
            }
        }
    }
}

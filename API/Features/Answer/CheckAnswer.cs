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
            public AnswerRequest Answer { get; set; }
            public DataModel.Models.Localization.Locale Locale { get; set; }
        }

        public class AnswerRequest
        {
            public Guid QuestionId { get; set; }
            public Guid AnswerId { get; set; }
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
                RuleFor(query => query.Answer).NotNull();
                RuleFor(query => query.Locale).IsInEnum();
                RuleFor(query => query.Answer.QuestionId).NotEmpty().When(q => q.Answer != null);
                RuleFor(query => query.Answer.AnswerId).NotEmpty().When(q => q.Answer != null);
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
                var correctAnswers = await GetCorrectAnswers(message.Answer.QuestionId);

                if (correctAnswers.Count == 0)
                {
                    throw new ValidationException($"No correct answers for {nameof(message.Answer.QuestionId)} '{message.Answer.QuestionId}'");
                }

                var text = await GetQuestionText(message.Answer.QuestionId, message.Locale);

                var result = new Result
                {
                    IsCorrect = correctAnswers.Any(answer => answer.Id == message.Answer.AnswerId),
                    CorrectAnswers = correctAnswers,
                    Text = text
                };

                return result;
            }

            private async Task<string> GetQuestionText(Guid questionId, DataModel.Models.Localization.Locale locale)
            {
                var query = from answer in _db.Answers
                            join questionLocalization in _db.QuestionLocalizations on answer.QuestionId equals questionLocalization.QuestionId
                            join localization in _db.Localizations on questionLocalization.LocalizationId equals localization.Id
                            where answer.QuestionId == questionId && questionLocalization.QuestionType == DataModel.Models.QuestionType.Final
                            where localization.Locale == locale
                            select localization.Text;

                var result = await query.FirstOrDefaultAsync();

                return result;
            }

            private async Task<ICollection<Answer>> GetCorrectAnswers(Guid questionId)
            {
                var query = from question in _db.Questions
                            join answer in _db.Answers on question.Id equals answer.QuestionId
                            where question.Id == questionId && answer.IsCorrect
                            select new Answer
                            {
                                Id = answer.Id
                            };

                var result = await query.ToListAsync();

                return result;
            }
        }
    }
}

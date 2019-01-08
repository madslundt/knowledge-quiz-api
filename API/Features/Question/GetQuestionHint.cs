using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using DataModel.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Question
{
    public class GetQuestionHint
    {
        public class Query : IRequest<Result>
        {
            public DataModel.Models.Localization.Locale Locale { get; set; }
            public Guid QuestionId { get; set; }
        }

        public class Result
        {
            public string Text { get; set; }
        }

        public class GetQuestionHintValidator : AbstractValidator<Query>
        {
            public GetQuestionHintValidator()
            {
                RuleFor(query => query.Locale).IsInEnum();
                RuleFor(query => query.QuestionId).NotEmpty();
            }
        }


        public class GetQuestionHintHandler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;

            public GetQuestionHintHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var hint = await GetHint(message.QuestionId, message.Locale);

                if (string.IsNullOrWhiteSpace(hint))
                {
                    throw new ArgumentNullException($"Could not find {nameof(message.QuestionId)} '{message.QuestionId}'");
                }

                var result = new Result
                {
                    Text = hint
                };

                return result;
            }

            private async Task<string> GetHint(Guid questionId, DataModel.Models.Localization.Locale locale)
            {
                var query = from questionLocalization in _db.QuestionLocalizations
                    join localization in _db.Localizations on questionLocalization.LocalizationId equals localization.Id
                    where questionLocalization.QuestionId == questionId && localization.Locale == locale && questionLocalization.QuestionType == QuestionType.Question
                    select localization.Text;

                var result = await query.FirstOrDefaultAsync();

                return result;
            }
        }
    }
}

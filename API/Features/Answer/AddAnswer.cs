using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using DataModel.Models.User;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Question
{
    public class AddAnswer
    {
        public class Command : IRequest
        {
            public Guid AnswerId { get; set; }
            public Guid UserId { get; set; }
        }

        public class AddAnswerValidator : AbstractValidator<Command>
        {
            public AddAnswerValidator()
            {
                RuleFor(question => question.AnswerId).NotEmpty();
                RuleFor(question => question.UserId).NotEmpty();
            }
        }


        public class AddAnswerHandler : IRequestHandler<Command>
        {
            private readonly DatabaseContext _db;

            public AddAnswerHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var doesAnswerExist = await DoesAnswerExist(message.AnswerId);

                if (!doesAnswerExist)
                {
                    throw new ArgumentNullException($"Could not find {nameof(message.AnswerId)} '{message.AnswerId}'");
                }

                await AddAnswer(message.AnswerId, message.UserId).ConfigureAwait(false);

                await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return Unit.Value;
            }

            private async Task<bool> DoesAnswerExist(Guid answerId)
            {
                var query = from answer in _db.Answers
                            where answer.Id == answerId
                            select answer.Id;

                var result = await query.AnyAsync();

                return result;
            }

            private async Task AddAnswer(Guid answerId, Guid userId)
            {
                var userAnswer = new UserAnswer
                {
                    AnswerId = answerId,
                    UserId = userId
                };

                _db.UserAnswers.Add(userAnswer);
            }
        }
    }
}

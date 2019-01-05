using System;
using System.Threading;
using System.Threading.Tasks;
using API.Features.Answer;
using DataModel;
using DataModel.Models.User;
using FluentValidation;
using MediatR;

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
            private readonly IMediator _mediator;

            public AddAnswerHandler(DatabaseContext db, IMediator mediator)
            {
                _db = db;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var answer = await _mediator.Send(new GetAnswers.Query
                {
                    AnswerId = message.AnswerId
                });

                if (answer is null)
                {
                    throw new ArgumentNullException($"Could not find {nameof(message.AnswerId)} '{message.AnswerId}'");
                }

                await AddAnswer(message.AnswerId, message.UserId).ConfigureAwait(false);

                await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return Unit.Value;
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

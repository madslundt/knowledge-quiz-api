using DataModel;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Features.Question
{
    public class MarkUserQuestion
    {
        public class Command : IRequest
        {
            public Guid UserId { get; set; }
            public ICollection<Question> Questions { get; set; }
        }

        public class Question
        {
            public Guid QuestionId { get; set; }
            public bool HintUsed { get; set; }
        }

        public class MarkUserQuestionValidator : AbstractValidator<Command>
        {
            public MarkUserQuestionValidator()
            {
                RuleFor(command => command.UserId).NotEmpty();
                RuleFor(command => command.Questions).NotEmpty();
                RuleForEach(command => command.Questions).Must(question => question.QuestionId != Guid.Empty);
            }
        }


        public class MarkUserQuestionHandler : IRequestHandler<Command>
        {
            private readonly DatabaseContext _db;

            public MarkUserQuestionHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var questionsToAdd = PrepareQuestions(message.UserId, message.Questions);

                await _db.AddRangeAsync(questionsToAdd);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }

            private ICollection<DataModel.Models.User.UserQuestion> PrepareQuestions(Guid userId, ICollection<Question> questions)
            {
                var result = questions.Select(question => new DataModel.Models.User.UserQuestion
                {
                    QuestionId = question.QuestionId,
                    UserId = userId,
                    HintUsed = question.HintUsed
                }).ToList();

                return result;
            }
        }
    }
}

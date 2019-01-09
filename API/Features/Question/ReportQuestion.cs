using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Features.Question
{
    public class ReportQuestion
    {
        public class Command : IRequest
        {
            public Guid UserId { get; set; }
            public Guid QuestionId { get; set; }
        }

        public class ReportQuestionValidator : AbstractValidator<Command>
        {
            public ReportQuestionValidator()
            {
                RuleFor(command => command).NotNull();
                RuleFor(command => command.UserId).NotEmpty();
                RuleFor(command => command.QuestionId).NotEmpty();
            }
        }


        public class ReportQuestionHandler : IRequestHandler<Command>
        {
            private readonly DatabaseContext _db;

            public ReportQuestionHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var questionReport = await PrepareQuestionReport(message.UserId, message.QuestionId);

                await _db.QuestionReport.AddAsync(questionReport);
                await _db.SaveChangesAsync();

                return Unit.Value;
            }

            private async Task<QuestionReport> PrepareQuestionReport(Guid userId, Guid questionId)
            {
                var questionReport = new QuestionReport
                {
                    UserId = userId,
                    QuestionId = questionId
                };

                return questionReport;
            }
        }
    }
}

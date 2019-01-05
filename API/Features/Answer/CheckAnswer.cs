using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Features.Question;
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
        }

        public class Result
        {
            public bool IsCorrect { get; set; }
            public Guid CorrectAnswerId { get; set; }
        }

        public class Answer
        {
            public Guid Id { get; set; }
        }

        public class CheckAnswerValidator : AbstractValidator<Query>
        {
            public CheckAnswerValidator()
            {
                RuleFor(question => question.AnswerId).NotEmpty();
            }
        }


        public class CheckAnswerHandler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;
            private readonly IMediator _mediator;

            public CheckAnswerHandler(DatabaseContext db, IMediator mediator)
            {
                _db = db;
                _mediator = mediator;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var questionId = await GetQuestionId(message.AnswerId);

                if (questionId is null)
                {
                    throw new ArgumentNullException($"Could not find {nameof(message.AnswerId)} '{message.AnswerId}'");
                }

                var question = await _mediator.Send(new GetQuestion.Query
                {
                    QuestionId = questionId.Value
                });

                var correctAnswer = question.Answers.FirstOrDefault(answer => answer.IsCorrect);

                var result = new Result
                {
                    IsCorrect = correctAnswer.Id == message.AnswerId,
                    CorrectAnswerId = correctAnswer.Id
                };

                return result;
            }

            private async Task<Guid?> GetQuestionId(Guid answerId)
            {
                var query = from answer in _db.Answers
                    where answer.Id == answerId
                    select answer.QuestionId;

                var result = await query.FirstOrDefaultAsync();

                return result;
            }
        }
    }
}

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
    public class GetAnswers
    {
        public class Query : IRequest<Result>
        {
            public Guid AnswerId { get; set; }
        }

        public class Result
        {
            public ICollection<Answer> Answers { get; set; }
        }

        public class Answer
        {
            public Guid Id { get; set; }
        }

        public class GetAnswerValidator : AbstractValidator<Query>
        {
            public GetAnswerValidator()
            {
                RuleFor(question => question.AnswerId).NotEmpty();
            }
        }


        public class GetAnswerHandler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;

            public GetAnswerHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var answers = await Get(message.AnswerId);

                var result = new Result
                {
                    Answers = answers
                };

                return result;
            }

            private async Task<ICollection<Answer>> Get(Guid answerId)
            {
                var query = from answer in _db.Answers
                    where answer.Id == answerId
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

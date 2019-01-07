using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Question
{
    public class GetQuestion
    {
        public class Query : IRequest<Result>
        {
            public Guid QuestionId { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }

            public ICollection<Answer> Answers { get; set; }
        }

        public class Answer
        {
            public Guid Id { get; set; }
            public bool IsCorrect { get; set; }
        }

        public class GetQuestionValidator : AbstractValidator<Query>
        {
            public GetQuestionValidator()
            {
                RuleFor(question => question.QuestionId).NotEmpty();
            }
        }


        public class GetQuestionHandler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;

            public GetQuestionHandler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var result = await Get(message.QuestionId);

                return result;
            }

            private async Task<Result> Get(Guid questionId)
            {   
                var query = from question in _db.Questions
                        join answer in _db.Answers on question.Id equals answer.QuestionId
                        where question.Id == questionId
                        group answer by question
                        into gr
                        select new Result
                        {
                            Id = gr.Key.Id,
                            Answers = gr.Select(a => new Answer
                            {
                                Id = a.Id,
                                IsCorrect = a.IsCorrect
                            }).ToList()
                        };

                var result = await query.FirstOrDefaultAsync();

                return result;
            }
        }
    }
}

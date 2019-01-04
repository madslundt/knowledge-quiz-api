using AutoMapper;
using DataModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Features.Question
{
    public class GetQuestions
    {
        public class Query : IRequest<Result>
        {
            public DataModel.Models.Localization.Locale Locale { get; set; }
            public string DeviceId { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
            public ICollection<Answer> Answers { get; set; }
        }

        public class Answer
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
        }

        public class MappingQuestion : Profile
        {
            public MappingQuestion()
            {
                CreateMap<DataModel.Models.Question.Question, Result>(MemberList.Source);
            }
        }

        public class GetQuestionsHandler : IRequestHandler<Query, Result>
        {
            private readonly DatabaseContext _db;
            private readonly IMapper _mapper;

            public GetQuestionsHandler(DatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {

            }
        }
    }
}

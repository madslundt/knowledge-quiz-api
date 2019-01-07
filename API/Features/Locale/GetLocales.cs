using DataModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Features.Locale
{
    public class GetLocales
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public ICollection<Locale> Locales { get; set; }
        }

        public class Locale
        {
            public DataModel.Models.Localization.Locale Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
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
                var locales = await Get();

                var result = new Result
                {
                    Locales = locales
                };

                return result;
            }

            private async Task<ICollection<Locale>> Get()
            {
                var query = from locale in _db.LocaleReferences
                            select new Locale
                            {
                                Id = locale.Id,
                                Code = locale.Code,
                                Name = locale.Name
                            };

                var result = await query.ToListAsync();

                return result;
            }
        }
    }
}

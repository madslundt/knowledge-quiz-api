using System;
using System.Threading;
using System.Threading.Tasks;
using API.Infrastructure.Identity;
using FluentValidation;
using MediatR;

namespace API.Features.User
{
    public class GetUserToken
    {
        public class Query : IRequest<Result>
        {
            public string UniqueId { get; set; }
        }

        public class Result
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
        }

        public class GetUserValidator : AbstractValidator<Query>
        {
            public GetUserValidator()
            {
                RuleFor(query => query).NotNull();
                RuleFor(query => query.UniqueId).NotEmpty();
            }
        }


        public class GetUserHandler : IRequestHandler<Query, Result>
        {
            private readonly IUserService _userService;

            public GetUserHandler(IUserService userService)
            {
                _userService = userService;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var token = await _userService.Authenticate(message.UniqueId);

                if (token is null)
                {
                    throw new ArgumentNullException($"Could not find {nameof(message.UniqueId)} '{message.UniqueId}'");
                }

                var result = new Result
                {
                    Token = token.Token,
                    Expiration = token.Expiration
                };

                return result;
            }
        }
    }
}

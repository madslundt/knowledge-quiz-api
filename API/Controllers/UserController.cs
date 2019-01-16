using API.Features.User;
using API.Infrastructure.MessageQueue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> CreateUser([FromBody] AddUser.Command user)
        {
            await _mediator.Send(user);

            return Ok();
        }

        [HttpGet, Route("{userId}")]
        [ProducesResponseType(200, Type = typeof(GetUser.Result))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [Authorize]
        public async Task<IActionResult> GetUser([FromRoute] Guid userId)
        {
            var result = await _mediator.Send(new GetUser.Query
            {
                UserId = userId
            });

            return Ok(result);
        }

        [HttpPost, Route("signin")]
        [ProducesResponseType(200, Type = typeof(GetUserToken.Result))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserToken([FromBody] GetUserToken.UserRequest user)
        {
            var result = await _mediator.Send(new GetUserToken.Query
            {
                User = user
            });

            return Ok(result);
        }

        [HttpPost, Route("metadata")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize]
        public async Task<IActionResult> AddMetadata([FromBody] AddMetadata.Metadata metadata)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            _mediator.Enqueue(new AddMetadata.Command
            {
                UserId = Guid.Parse(userId),
                Metadata = metadata
            });

            return Ok();
        }
    }
}

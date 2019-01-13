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
        public async Task<IActionResult> CreateUser([FromBody] AddUser.Command user)
        {
            await _mediator.Send(user);

            return Ok();
        }

        [HttpGet, Route("{userId}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid userId)
        {
            var result = await _mediator.Send(new GetUser.Query
            {
                UserId = userId
            });

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByUniqueId([FromBody] GetUserToken.Query user)
        {
            var result = await _mediator.Send(user);

            return Ok(result);
        }

        [HttpPost, Route("metadata")]
        [Authorize]
        public async Task<IActionResult> AddMetadata([FromBody] AddMetadata.Metadata metadata)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            _mediator.Enqueue(new AddMetadata.Command
            {
                UserId = Guid.Parse(userId),
                Metadata = metadata
            });

            return Ok();
        }
    }
}

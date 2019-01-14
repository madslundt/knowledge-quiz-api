using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Features.Answer;
using API.Infrastructure.MessageQueue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/answers")]
    [Authorize]
    public class AnswerController : Controller
    {
        private readonly IMediator _mediator;

        public AnswerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("{answerId}")]
        [ProducesResponseType(200, Type = typeof(CheckAnswer.Result))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> AddAnswer([FromHeader] DataModel.Models.Localization.Locale locale, [FromRoute] Guid answerId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            var result = await _mediator.Send(new CheckAnswer.Query
            {
                AnswerId = answerId,
                Locale = locale
            });
            
            _mediator.Enqueue(new AddAnswer.Command
            {
                AnswerId = answerId,
                UserId = Guid.Parse(userId)
            });

            return Ok(result);
        }
    }
}

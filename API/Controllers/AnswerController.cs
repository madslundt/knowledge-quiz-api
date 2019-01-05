using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Features.Answer;
using API.Features.Question;
using API.Infrastructure.MessageQueue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/answer")]
    public class AnswerController : Controller
    {
        private readonly IMediator _mediator;

        public AnswerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("{answerId}")]
        [Authorize]
        public async Task<IActionResult> AddAnswer([FromRoute] Guid answerId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(new CheckAnswer.Query
            {
                AnswerId = answerId
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

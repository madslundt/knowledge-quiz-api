using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Features.Question;
using API.Infrastructure.MessageQueue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/questions")]
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly IMediator _mediator;

        public QuestionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestions([FromHeader] DataModel.Models.Localization.Locale locale, [FromQuery] int limit = 20)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            var result = await _mediator.Send(new GetQuestions.Query
            {
                Limit = 20,
                UserId = Guid.Parse(userId),
                Locale = locale
            });

            return Ok(result);
        }

        [HttpPut, Route("{questionId}/mark")]
        public IActionResult MarkQuestion([FromRoute] Guid questionId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            _mediator.Enqueue(new MarkUserQuestion.Command
            {
                Questions = new List<MarkUserQuestion.Question>
                {
                    new MarkUserQuestion.Question
                    {
                        QuestionId = questionId
                    }
                },
                UserId = Guid.Parse(userId)
            });

            return Ok();
        }

        [HttpGet, Route("{questionId}/hint")]
        public async Task<IActionResult> GetQuestionHint([FromHeader] DataModel.Models.Localization.Locale locale, [FromRoute] Guid questionId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            var result = await _mediator.Send(new GetQuestionHint.Query
            {
                QuestionId = questionId,
                Locale = locale
            });

            _mediator.Enqueue(new MarkUserQuestion.Command
            {
                UserId = Guid.Parse(userId),
                Questions = new List<MarkUserQuestion.Question>
                {
                    new MarkUserQuestion.Question
                    {
                        QuestionId = questionId,
                        HintUsed = true
                    }
                }
            });

            return Ok(result);
        }

        [HttpPost, Route("{questionId}/report")]
        public async Task<IActionResult> ReportQuestion([FromRoute] Guid questionId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            await _mediator.Send(new ReportQuestion.Command
            {
                UserId = Guid.Parse(userId),
                QuestionId = questionId
            });

            return Ok();
        }
    }
}
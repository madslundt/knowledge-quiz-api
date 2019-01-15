using API.Features.Locale;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/locales")]
    public class LocaleController : Controller
    {
        private readonly IMediator _mediator;

        public LocaleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(GetLocales.Result))]
        public async Task<IActionResult> GetLocales()
        {
            var result = await _mediator.Send(new GetLocales.Query());

            return Ok(result);
        }
    }
}

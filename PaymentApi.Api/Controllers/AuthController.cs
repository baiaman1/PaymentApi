using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentApi.Application.Features.Auth;

namespace PaymentApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _mediator;

        public AuthController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand cmd)
        {
            var res = await _mediator.Send(cmd);
            return Ok(res);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var header = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(header)) return Unauthorized();

            var token = header.Replace("Bearer ", "").Trim();
            var cmd = new LogoutCommand { Token = token };
            var ok = await _mediator.Send(cmd);
            if (!ok) return Unauthorized();
            return Ok();
        }
    }
}

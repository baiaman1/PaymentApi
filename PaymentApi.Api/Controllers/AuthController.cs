using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentApi.Application.DTOs;
using PaymentApi.Application.Features.Auth;
using PaymentApi.Application.Interfaces;

namespace PaymentApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _mediator; // MediatR

        public AuthController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand cmd)
        {
            var res = await _mediator.Send(cmd);
            return Ok(res);
        }

        [HttpPost("logout")]
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

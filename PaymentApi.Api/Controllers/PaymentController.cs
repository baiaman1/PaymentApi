using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentApi.Application.Features.Payments;
using System.Security.Claims;

namespace PaymentApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ISender _mediator;

        public PaymentController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Pay()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) ||
                !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var result = await _mediator.Send(new MakePaymentCommand { Token = token });

            if (!result.Success)
                return BadRequest(new { error = "Insufficient balance", balance = result.NewBalance });

            return Ok(new { balance = result.NewBalance });
        }
    }
}

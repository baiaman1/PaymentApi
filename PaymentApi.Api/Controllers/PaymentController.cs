using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentApi.Application.Features.Payments;

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
            var header = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(header)) return Unauthorized();

            var token = header.Replace("Bearer ", "").Trim();
            var cmd = new MakePaymentCommand { Token = token };
            var res = await _mediator.Send(cmd);
            if (res == null) return BadRequest(new { error = "Insufficient balance" });
            return Ok(res);
        }
    }
}

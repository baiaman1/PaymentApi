using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentApi.Application.Features.Payments
{
    public class MakePaymentCommand : IRequest<MakePaymentResult>
    {
        public string Token { get; set; } = null!;
    }

    public class MakePaymentResult
    {
        public decimal NewBalance { get; set; }
    }
}

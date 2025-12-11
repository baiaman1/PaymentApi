using MediatR;

namespace PaymentApi.Application.Features.Payments
{
    public class MakePaymentCommand : IRequest<MakePaymentResult>
    {
        public string Token { get; set; } = null!;
    }

    public class MakePaymentResult
    {
        public bool Success { get; set; }
        public decimal NewBalance { get; set; }
    }

}

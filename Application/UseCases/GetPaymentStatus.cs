using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Domain.Enums;

namespace Application.UseCases
{
    public class GetPaymentStatus : IGetPaymentStatus
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentStatus(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentStatusDto?> ExecuteAsync(int orderId)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            if (payment == null)
                return null;

            return new PaymentStatusDto
            {
                OrderId = payment.OrderId,
                Status = Enum.GetName(typeof(PaymentStatus), payment.PaymentStatus) ?? PaymentStatus.Pending.ToString(),
                PaymentDateProcessed = payment.PaymentDateProcessed ?? null
            };
        }
    }
}

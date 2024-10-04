using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Domain.Enums;

namespace Application.UseCases
{
    public class GetApprovedPayments : IGetApprovedPayments
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetApprovedPayments(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<PaymentStatusDto>> ExecuteAsync(int limit)
        {
            var approvedPayments = await _paymentRepository.GetApprovedPaymentsAsync(limit);
            var paymentResponses = new List<PaymentStatusDto>();

            foreach (var payment in approvedPayments)
            {
                var response = new PaymentStatusDto
                {
                    OrderId = payment.OrderId,
                    OrderNumber = payment.OrderNumber,
                    Status = Enum.GetName(typeof(PaymentStatus), payment.PaymentStatus) ?? PaymentStatus.Pending.ToString(),
                    PaymentDateProcessed = payment.PaymentDateProcessed
                };
                paymentResponses.Add(response);
            }

            return paymentResponses;
                    
        }
    }
}

using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases
{
    public class HandlePaymentWebhook : IHandlePaymentWebhook
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMercadoPagoService _mercadoPagoService;

        public HandlePaymentWebhook(IPaymentRepository paymentRepository, IMercadoPagoService mercadoPagoService)
        {
            _paymentRepository = paymentRepository;
            _mercadoPagoService = mercadoPagoService;
        }

        public async Task<string> ExecuteAsync(WebhookDto request)
        {          
            Payment PaymentData = null;
            switch (request.Type)
            {
                case "payment":
                    PaymentMPResponseDto? payment = await _mercadoPagoService.GetPaymentAsync(request.Data.Id.ToString());

                    if (payment != null && payment.Order != null && payment.Order.Id.HasValue)
                    {
                        PaymentData = new Payment
                        {
                            OrderNumber = payment.ExternalReference.Replace("ORD", ""),
                            PaymentMethod = payment.PaymentTypeId,
                            PaymentDate = DateTime.UtcNow
                        };

                        if (Enum.TryParse(payment.Status, true, out PaymentStatus paymentStatus))
                        {
                            PaymentData.PaymentStatus = (int)paymentStatus;
                        }
                        else
                            throw new Exception($"Status '{payment.Status}' não é válido.");
                    };
                    break;
                default:
                    return "";
            }

            await _paymentRepository.UpdatePaymentStatusAsync(PaymentData);

            return "";
        }
    }
}

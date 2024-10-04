using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface ICreatePayment
    {
        Task<string?> ExecuteAsync(PaymentRequestDto paymentRequest);
    }
}

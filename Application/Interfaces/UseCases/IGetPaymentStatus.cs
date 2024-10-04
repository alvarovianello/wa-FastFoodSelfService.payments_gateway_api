using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface IGetPaymentStatus
    {
        Task<PaymentStatusDto?> ExecuteAsync(int orderId);
    }
}

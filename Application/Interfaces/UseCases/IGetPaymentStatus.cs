using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.UseCases
{
    public interface IGetPaymentStatus
    {
        Task<PaymentStatusDto?> ExecuteAsync(int orderId);
    }
}

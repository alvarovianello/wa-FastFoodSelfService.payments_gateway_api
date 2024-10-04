using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByOrderIdAsync(int orderId);
        Task UpdatePaymentStatusAsync(Payment payment);
        Task AddAsync(Payment payment);
        Task<IEnumerable<Payment>> GetApprovedPaymentsAsync(int limit);
    }
}

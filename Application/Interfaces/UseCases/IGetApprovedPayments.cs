using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface IGetApprovedPayments
    {
        Task<IEnumerable<PaymentStatusDto>> ExecuteAsync(int limit);
    }
}

using Application.DTOs;

namespace Application.Interfaces.ExternalServices
{
    public interface IOrderService
    {
        Task<OrderReponseDto?> GetOrderByOrderNumberAsync(string orderNumber);
    }
}

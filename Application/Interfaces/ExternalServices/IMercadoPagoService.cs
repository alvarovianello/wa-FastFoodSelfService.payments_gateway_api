using Application.DTOs;

namespace Application.Interfaces.ExternalServices
{
    public interface IMercadoPagoService
    {
        Task<QrCodeResponseDto> GenerateQrCodeMPAsync(OrderReponseDto order);
        Task<bool> GenerateOrderMPAsync(OrderReponseDto order);
        Task<PaymentMPResponseDto?> GetPaymentAsync(string paymentId);
        Task<MerchantOrdersMPResponseDto?> GetMerchantOrderAsync(string merchantOrder);
    }
}

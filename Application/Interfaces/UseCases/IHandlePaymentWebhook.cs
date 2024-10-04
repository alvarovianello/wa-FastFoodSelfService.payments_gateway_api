using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface IHandlePaymentWebhook
    {
        Task<string> ExecuteAsync(WebhookDto request);
    }
}

using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class WebhookDto
    {
        public string? Type { get; set; }           // Tipo de evento (ex: payment)
        public WebhookDataDto? Data { get; set; }   // Dados específicos do evento
    }

    public class WebhookDataDto
    {
        public string? Id { get; set; }             // ID do recurso (ex: pagamento)
    }
}

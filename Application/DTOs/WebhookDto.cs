using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class WebhookDto
    {
        public long? Id { get; set; }              // ID da notificação

        [JsonPropertyName("live_mode")]
        public bool? LiveMode { get; set; }         // Ambiente (produção ou sandbox)
        public string? Type { get; set; }           // Tipo de evento (ex: payment)

        [JsonPropertyName("date_created")]
        public DateTime? DateCreated { get; set; }  // Data de criação do evento

        [JsonPropertyName("application_id")]
        public string? ApplicationId { get; set; }  // ID da aplicação que gerou a notificação

        [JsonPropertyName("user_id")]
        public long? UserId { get; set; }           // ID do usuário relacionado ao evento

        [JsonPropertyName("api_version")]
        public string? ApiVersion { get; set; }     // Versão da API Mercado Pago
        public string? Action { get; set; }         // Ação do evento (ex: payment.created)
        public WebhookDataDto? Data { get; set; }   // Dados específicos do evento
    }

    public class WebhookDataDto
    {
        public string? Id { get; set; }             // ID do recurso (ex: pagamento)
    }
}

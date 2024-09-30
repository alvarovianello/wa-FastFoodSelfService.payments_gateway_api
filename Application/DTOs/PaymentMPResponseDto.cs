using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class PaymentMPResponseDto
    {
        public long Id { get; set; }

        [JsonPropertyName("external_reference")]
        public string? ExternalReference { get; set; }

        public string? Status { get; set; }

        public OrderMPDto? Order { get; set; }

        [JsonPropertyName("payment_type_id")]
        public string? PaymentTypeId { get; set; }
    }

    public class OrderMPDto
    {
        public long? Id { get; set; }
    }
}

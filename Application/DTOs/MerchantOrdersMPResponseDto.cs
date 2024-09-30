using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class MerchantOrdersMPResponseDto
    {
        public int Id { get; set; }
        public string? Status { get; set; }

        [JsonPropertyName("external_reference")]
        public string? ExternalReference { get; set; }

        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }

        public IEnumerable<PaymentsMPDto> Payments { get; set; } = new List<PaymentsMPDto>();
        public IEnumerable<ShipmentsMPDto> Shipments { get; set; } = new List<ShipmentsMPDto>();

    }

    public class PaymentsMPDto
    {
        public int Id { get; set; }
        public string? Status { get; set; }

        [JsonPropertyName("transaction_amount")]
        public decimal? TransactionAmount { get; set; }

    }
    public class ShipmentsMPDto
    {
        public string? Status { get; set; }

    }
}

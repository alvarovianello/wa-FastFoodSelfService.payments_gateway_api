using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class QrCodeResponseDto
    {
        [JsonPropertyName("in_store_order_id")]
        public string? InStoreOrderId { get; set; }

        [JsonPropertyName("qr_data")]
        public string? QrData { get; set; }
    }
}

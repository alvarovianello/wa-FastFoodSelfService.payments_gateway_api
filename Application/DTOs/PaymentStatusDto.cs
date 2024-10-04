using Domain.Enums;

namespace Application.DTOs
{
    public class PaymentStatusDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? PaymentDateProcessed { get; set; }
    }
}

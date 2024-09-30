using Domain.Enums;

namespace Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string PaymentMethod { get; set; }
        public int PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public DateTime? PaymentDateProcessed { get; set; }
        public string InStoreOrderId { get; set; }
        public string QrData { get; set; }
    }
}

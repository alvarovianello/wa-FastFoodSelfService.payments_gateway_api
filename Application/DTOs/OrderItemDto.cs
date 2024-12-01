namespace Application.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceItem { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
    }
}

namespace Ecom.Core.DTOs
{
    public record OrderItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string MainImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
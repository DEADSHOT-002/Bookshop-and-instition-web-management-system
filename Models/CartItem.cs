namespace BookshopTuitionSystem.Models
{
    public class CartItem
    {
        public int ItemId { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Price * Quantity;
    }
}
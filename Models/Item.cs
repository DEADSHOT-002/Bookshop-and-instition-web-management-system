using System;
using System.ComponentModel.DataAnnotations;

namespace BookshopTuitionSystem.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = "";

        [Required]
        public string Brand { get; set; } = "";

        public int Quantity { get; set; }

        public string PurchaseReceiptNo { get; set; } = "";

        public DateTime DatePurchased { get; set; }

        public string Supplier { get; set; } = "";

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }
    }
}
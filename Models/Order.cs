using System;
using System.Collections.Generic;

namespace BookshopTuitionSystem.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string InvoiceNumber { get; set; } = "";
        public string CashierName { get; set; } = "";
        public string PaymentMethod { get; set; } = "Cash";

        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ChangeReturned { get; set; }

        public string Status { get; set; } = "Completed";
        public int UserId { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace FoodXYZ.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNum { get; set; }
        public int PriceTotal { get; set; }
        public int QtyTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<DetailInvoice> DetailInvoices { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
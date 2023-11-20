using System.Text.Json.Serialization;

namespace FoodXYZ.Models{
    public class DetailInvoice
    {
        public int Id { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
        public int ProductId { get; set; }
        [JsonIgnore]
        public Invoice Invoice { get; set; }
        public int InvoiceId { get; set; }
        public int Qty { get; set; }
        public int SubTotal { get; set; }
    }
}
namespace CozyComfort.Client.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? SellerName { get; set; }
        public int BlanketId { get; set; }
        public int Quantity { get; set; }
        public string? Status { get; set; }

        
        public string? AssignedDistributor { get; set; }
        public DateTime? ProcessedDate { get; set; }
    }
}
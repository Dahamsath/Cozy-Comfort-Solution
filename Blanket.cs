namespace CozyComfort.Client.Models
{
    public class Blanket
    {
        public int Id { get; set; }
        public string? ModelName { get; set; }
        public string? Material { get; set; }
        public decimal Price { get; set; }
        public int StockLevel { get; set; }

        
        public int ProductionCapacity { get; set; }
    }
}
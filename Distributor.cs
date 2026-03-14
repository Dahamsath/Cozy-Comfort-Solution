namespace CozyComfort.API.Models
{
    public class Distributor
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string ContactEmail { get; set; }
        public List<Blanket> AvailableStock { get; set; } = new List<Blanket>();
    }
}
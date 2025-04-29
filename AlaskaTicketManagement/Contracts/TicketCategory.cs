namespace AlaskaTicketManagement.Contracts
{
    public class TicketCategory
    {
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}

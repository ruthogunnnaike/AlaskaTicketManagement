namespace AlaskaTicketManagement.Contracts
{
    public class ReservationRequest
    {
        public int EventId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string UserEmail { get; set; } = string.Empty; 
    }
}

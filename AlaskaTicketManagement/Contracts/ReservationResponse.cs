namespace AlaskaTicketManagement.Contracts
{
    public class ReservationResponse
    {
        public int ReservationId { get; set; }
        public int EventId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

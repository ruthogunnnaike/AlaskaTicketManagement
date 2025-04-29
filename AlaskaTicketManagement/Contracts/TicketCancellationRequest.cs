namespace AlaskaTicketManagement.Contracts
{
    public class TicketCancellationRequest
    {
        public int ReservationId { get; set; }
        public string UserEmail { get; set; }
    }
}


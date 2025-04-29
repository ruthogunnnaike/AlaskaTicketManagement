namespace AlaskaTicketManagement.Models
{
    /// <summary>
    /// Temporarily holds tickets for a user.
    /// </summary>
    public class Reservation
    {
        public int Id { get; set; }
        public int EventId { get; set; }

        public List<Ticket> Tickets { get; set; } = new();

        public string UserEmail { get; set; } = string.Empty;
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(15); // reservation timeout
    }

}

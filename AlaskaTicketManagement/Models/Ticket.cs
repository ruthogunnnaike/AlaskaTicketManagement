using System.ComponentModel.DataAnnotations;

namespace AlaskaTicketManagement.Models
{
    public enum TicketStatus
    {
        Available,
        Reserved,
        Sold
    }

    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public string TicketType { get; set; } = "General Admission";
        public decimal Price { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Available;
        public DateTime? ReservationExpiresAt { get; set; }
    }
}
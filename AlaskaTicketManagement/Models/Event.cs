using System.ComponentModel.DataAnnotations;

namespace AlaskaTicketManagement.Models
{
    public class Event
    {
        public int Id { get; set; }

        [StringLength(25)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        public int VenueId { get; set; }

        /// <summary>
        /// Navigation exist
        /// </summary>
        public Venue Venue { get; set; } = null!;

        public DateTimeOffset Date { get; set; }  // For timezone adaptability

        public int TotalTickets { get; set; }

        public int TicketsAvailable { get; set; }
        /// <summary>
        /// List of Tickets Types. Navigation property
        /// </summary>
        public List<Ticket> Tickets { get; set; } = new();
    }
}

using System.ComponentModel.DataAnnotations;

namespace AlaskaTicketManagement.Contracts
{
    public class EventRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int VenueId { get; set; }

        [Required]
        public DateTimeOffset Date { get; set; }

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public List<TicketCategory> TicketCategories { get; set; } = new();

    }  
}

using AlaskaTicketManagement.Models;

namespace AlaskaTicketManagement.Contracts
{
    public class EventResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; } = string.Empty;

        public int VenueId { get; set; }
        public Venue Venue { get; set; } = null!;

        public int TicketsAvailable { get; set; }

        public List<TicketCategory> TicketTypes { get; set; } = new();
    }
}

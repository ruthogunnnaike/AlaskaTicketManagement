using Microsoft.EntityFrameworkCore;
using AlaskaTicketManagement.Models;
using AlaskaTicketManagement.Services;
using AlaskaTicketManagement.Contracts;

namespace AlaskaTicketManagement.Data
{
    public class SyntheticDataGenerator
    {
        private readonly IVenueService _venueService;
        private readonly IEventService _eventService;

        public SyntheticDataGenerator(IVenueService venueService, IEventService eventService)
        {
            _venueService = venueService;
            _eventService = eventService;
        }

        public async Task GenerateAsync(int venueCount = 3, int eventsPerVenue = 2)
        {
            for (int v = 1; v <= venueCount; v++)
            {
                var venueRequest = new VenueRequest
                {
                    Name = $"Venue {v}",
                    Location = $"City {v}",
                    Capacity = 100 + v * 50
                };

                var venue = await _venueService.CreateVenueAsync(venueRequest);

                for (int e = 1; e <= eventsPerVenue; e++)
                {
                    var eventRequest = new EventRequest
                    {
                        Name = $"Event {v}-{e}",
                        Description = $"This is Event {v}-{e}",
                        Date = DateTimeOffset.UtcNow.AddDays(e * 3),
                        VenueId = venue.VenueId,
                        TicketCategories = new List<TicketCategory>
                        {
                            new TicketCategory
                            {
                                Type = "General",
                                Price = 25,
                                Quantity = 50
                            }
                        }
                    };

                    await _eventService.CreateEventAsync(eventRequest);
                }
            }
        }
    }
}

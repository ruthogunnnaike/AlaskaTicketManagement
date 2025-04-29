using AlaskaTicketManagement.Contracts;
using AlaskaTicketManagement.Models;
using AlaskaTicketManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace AlaskaTicketManagement.Services
{
    public class EventService : IEventService
    {
        private readonly AlaskaConcertDbContext _context;

        public EventService(AlaskaConcertDbContext context)
        {
            _context = context;
        }

        public async Task<EventResponse> CreateEventAsync(EventRequest request)
        { 
            var newEvent = new Event
            {
                Name = request.Name,
                Description = request.Description,
                Date = request.Date,
                VenueId = request.VenueId,
                TicketsAvailable = request.TicketCategories.Sum(c => c.Quantity),
                TotalTickets = request.TicketCategories.Sum(c => c.Quantity),
            };
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            // Add ticket categories and create ticket for each category quantity
            var tickets = new List<Ticket>();
            foreach (var category in request.TicketCategories)
            {
                for (int i = 0; i < category.Quantity; i++) {
                    tickets.Add(new Ticket
                    {
                        EventId = newEvent.Id,
                        TicketType = category.Type,
                        Price = category.Price,
                        Status = TicketStatus.Available
                    });
                }
            }
            _context.Tickets.AddRange(tickets);
            await _context.SaveChangesAsync();

           return await GetEventAsync(newEvent.Id) ?? throw new Exception("Event creation failed");
        }

        public async Task<EventResponse?> GetEventAsync(int eventId)
        {
            var eventDetails = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventDetails == null) return null;

            return new EventResponse
            {
                Id = eventDetails.Id,
                Name = eventDetails.Name,
                Date = eventDetails.Date,
                Description = eventDetails.Description,
                VenueId = eventDetails.VenueId,
                Venue = eventDetails.Venue,
                TicketsAvailable = eventDetails.Tickets.Count(t => t.Status == TicketStatus.Available),
                TicketTypes = eventDetails.Tickets
                                .GroupBy(t => new { t.TicketType, t.Price })
                                .Select(g => new TicketCategory
                                {
                                    Type = g.Key.TicketType,
                                    Price = g.Key.Price,
                                    Quantity = g.Count(t => t.Status == TicketStatus.Available)
                                }).ToList()
            };
        }

        public async Task<bool> UpdateEventAsync(int eventId, EventRequest request)
        {
            var existing = await _context.Events
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == eventId) ?? throw new Exception("Event not found");

            existing.Name = request.Name;
            existing.Description = request.Description;
            existing.Date = request.Date;
            existing.VenueId = request.VenueId;

            // Process ticket updates
            foreach (var updatedTicket in request.TicketCategories)
            {
                var matchingTickets = existing.Tickets
                    .Where(t => t.TicketType == updatedTicket.Type && t.Status == TicketStatus.Available)
                    .ToList();

                int currentAvailable = matchingTickets.Count;

                // Update price of existing available tickets
                foreach (var ticket in matchingTickets)
                {
                    ticket.Price = updatedTicket.Price;
                }

                // Add additional tickets if quantity increased
                int extra = updatedTicket.Quantity - currentAvailable;
                if (extra > 0)
                {
                    var newTickets = Enumerable.Range(1, extra).Select(_ => new Ticket
                    {
                        EventId = existing.Id,
                        TicketType = updatedTicket.Type,
                        Price = updatedTicket.Price,
                        Status = TicketStatus.Available
                    });

                    _context.Tickets.AddRange(newTickets);
                }
            }

            // Update ticket availability counts
            existing.TotalTickets = request.TicketCategories.Sum(c => c.Quantity);
            existing.TicketsAvailable = existing.Tickets.Count(t => t.Status == TicketStatus.Available);

            await _context.SaveChangesAsync();
            return true;
        }
    }

}

using AlaskaTicketManagement.Models;
using AlaskaTicketManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using AlaskaTicketManagement.Contracts;
using System;
using Microsoft.AspNetCore.Mvc;

namespace AlaskaTicketManagement.Services
{
    public class TicketService : ITicketService
    {
        private readonly AlaskaConcertDbContext _context;
        private readonly TimeProvider _timeProvider;
        private readonly IPaymentService _paymentService;

        public TicketService(AlaskaConcertDbContext context, IPaymentService paymentService,
            TimeProvider timeProvider)
        {
            _context = context;
            _timeProvider = timeProvider;
            _paymentService = paymentService;
        }

        public async Task<ReservationResponse> ReserveTicketsAsync(ReservationRequest request)
        {
            // Validate if EventId exists
            var eventExists = await _context.Events.AnyAsync(e => e.Id == request.EventId);
            if (!eventExists)
            {
                throw new ArgumentException("Event specified in this reservation does not exist.");
            }

            var availableTickets = await _context.Tickets
                .Where(t =>
                    t.EventId == request.EventId &&
                    t.TicketType == request.TicketType &&
                    t.Status == TicketStatus.Available)
                .Take(request.Quantity)
                .ToListAsync();

            if (availableTickets.Count < request.Quantity)
            {
                throw new InvalidOperationException("Not enough tickets available.");
            }

            // Set ticket status to reserved
            foreach (var ticket in availableTickets)
            {
                ticket.Status = TicketStatus.Reserved;
                ticket.ReservationExpiresAt = DateTime.UtcNow.AddMinutes(15);
            }

            // Create reservation
            var reservation = new Reservation
            {
                EventId = request.EventId,
                UserEmail = request.UserEmail,
                ReservedAt = _timeProvider.GetUtcNow().UtcDateTime,
                ExpiresAt = _timeProvider.GetUtcNow().UtcDateTime.AddMinutes(15),
                Tickets = availableTickets
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return new ReservationResponse
            {
                ReservationId = reservation.Id,
                EventId = request.EventId,
                TicketType = request.TicketType,
                Quantity = request.Quantity,
                ExpiresAt = reservation.ExpiresAt
            };
        }

        public async Task<bool> PurchaseReservationAsync(TicketPurchaseRequest request)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Tickets)
                .FirstOrDefaultAsync(r => r.Id == request.ReservationId);

            if (reservation == null || reservation.ExpiresAt < _timeProvider.GetUtcNow().UtcDateTime)
            {
                return false;
            }

            // Process payment 
            var payment = await _paymentService.ProcessPaymentAsync(request);
            if (payment.Result is OkObjectResult result && result.Value is bool success) {
                if (success) {
                    foreach (var ticket in reservation.Tickets)
                    {
                        ticket.Status = TicketStatus.Sold;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }


        public async Task<bool> CancelReservationAsync(TicketCancellationRequest request)
        {
            var reservation = await _context.Reservations
                                .Include(r => r.Tickets)
                                .FirstOrDefaultAsync(r => r.Id == request.ReservationId
                                        && r.UserEmail == request.UserEmail);
            
            if (reservation == null || reservation.ExpiresAt < _timeProvider.GetUtcNow())
            {
                return false;
            }

            foreach (var ticket in reservation.Tickets)
            {
                ticket.Status = TicketStatus.Available;
                ticket.ReservationExpiresAt = null;
            }
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }
      
        public async Task<List<TicketCategory>> GetAvailableTicketsAsync(int eventId)
        {
            var eventDetails = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventDetails == null) return new();

            return eventDetails.Tickets
                .GroupBy(t => new { t.TicketType, t.Price })
                .Select(g => new TicketCategory
                {
                    Type = g.Key.TicketType,
                    Price = g.Key.Price,
                    Quantity = g.Count(t => t.Status == TicketStatus.Available)
                }).ToList();
        }
    }

}

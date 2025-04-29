using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AlaskaTicketManagement.Models;
using AlaskaTicketManagement.Data;

namespace AlaskaTicketManagement.Services
{
    public class ReservationService : BackgroundService
    {
        // TODO - Make reservation holds to be configurable - release ticket reservations after a configurable value
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReservationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(2);

        public ReservationService(IServiceScopeFactory scopeFactory, ILogger<ReservationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Expiration Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AlaskaConcertDbContext>();

                    var now = DateTime.UtcNow;

                    var expiredReservations = await dbContext.Tickets
                        .Where(t => t.Status == TicketStatus.Reserved && t.ReservationExpiresAt <= now)
                        .ToListAsync(stoppingToken);

                    if (expiredReservations.Any())
                    {
                        foreach (var ticket in expiredReservations)
                        {
                            ticket.Status = TicketStatus.Available;
                            ticket.ReservationExpiresAt = null;
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation($"Released {expiredReservations.Count} expired reservations.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while expiring reservations.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
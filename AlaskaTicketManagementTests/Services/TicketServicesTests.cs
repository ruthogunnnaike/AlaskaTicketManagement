using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlaskaTicketManagement.Contracts;
using AlaskaTicketManagement.Data;
using AlaskaTicketManagement.Models;
using AlaskaTicketManagement.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AlaskaTicketManagementTests.Services
{
    public class TicketServiceTests
    {
        private async Task<AlaskaConcertDbContext> GetDbContextWithTestData()
        {
            var options = new DbContextOptionsBuilder<AlaskaConcertDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new AlaskaConcertDbContext(options);

            var testEvent = new Event { Id = 1, Name = "Test Event" };
            var tickets = Enumerable.Range(1, 5).Select(i =>
                new Ticket
                {
                    Id = i,
                    EventId = 1,
                    TicketType = "VIP",
                    Status = TicketStatus.Available
                }).ToList();

            context.Events.Add(testEvent);
            context.Tickets.AddRange(tickets);
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task ReserveTicketsAsync_WithValidRequest_ShouldReserveTickets()
        {
            // Arrange
            var dbContext = await GetDbContextWithTestData();

            var mockPaymentService = new Mock<IPaymentService>();
            var mockTimeProvider = new Mock<TimeProvider>();
            var now = DateTimeOffset.UtcNow;

            mockTimeProvider.Setup(tp => tp.GetUtcNow()).Returns(now);

            var service = new TicketService(dbContext, mockPaymentService.Object, mockTimeProvider.Object);

            var request = new ReservationRequest
            {
                EventId = 1,
                Quantity = 3,
                TicketType = "VIP",
                UserEmail = "test@example.com"
            };

            // Act
            var result = await service.ReserveTicketsAsync(request);

            // Assert
            Assert.Equal(request.EventId, result.EventId);
            Assert.Equal(request.Quantity, result.Quantity);
            Assert.Equal("VIP", result.TicketType);
            Assert.True(dbContext.Tickets.Count(t => t.Status == TicketStatus.Reserved) == 3);
        }

        [Fact]
        public async Task ReserveTicketsAsync_WithNonExistingEvent_ShouldThrow()
        {
            var dbContext = await GetDbContextWithTestData();
            var mockTimeProvider = new Mock<TimeProvider>();
            var mockPaymentService = new Mock<IPaymentService>();

            var service = new TicketService(dbContext, mockPaymentService.Object, mockTimeProvider.Object);
            var request = new ReservationRequest { EventId = 999, Quantity = 1, TicketType = "VIP", UserEmail = "email@example.com" };

            await Assert.ThrowsAsync<ArgumentException>(() => service.ReserveTicketsAsync(request));
        }

        [Fact]
        public async Task ReserveTicketsAsync_WhenNotEnoughTickets_ShouldThrow()
        {
            var dbContext = await GetDbContextWithTestData();
            var mockTimeProvider = new Mock<TimeProvider>();
            var mockPaymentService = new Mock<IPaymentService>();

            var service = new TicketService(dbContext, mockPaymentService.Object, mockTimeProvider.Object);
            var request = new ReservationRequest { EventId = 1, Quantity = 10, TicketType = "VIP", UserEmail = "email@example.com" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReserveTicketsAsync(request));
        }
    }
}

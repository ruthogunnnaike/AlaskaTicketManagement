
using AlaskaTicketManagement.Data;
using AlaskaTicketManagement.Models;
using AlaskaTicketManagement.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AlaskaTicketManagement.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AlaskaTicketManagementTests.Services
{

    public class EventServiceTests
    {
        private AlaskaConcertDbContext GetDbContextVenueOnly()
        {
            var options = new DbContextOptionsBuilder<AlaskaConcertDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AlaskaConcertDbContext(options);

            context.Venues.Add(new Venue { Id = 1, Name = "Test Venue 1", Capacity = 100 });

            context.SaveChanges();
            return context;
        }
        private AlaskaConcertDbContext GetDbContextVenueAndEvent()
        {
            var options = new DbContextOptionsBuilder<AlaskaConcertDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AlaskaConcertDbContext(options);

            context.Venues.Add(new Venue { Id = 1, Name = "Test Venue 1", Capacity = 100 });
            context.Venues.Add(new Venue { Id = 2, Name = "Test Venue 2", Capacity = 50 });
            
            context.Events.Add(new Event
            {
                Id = 1,
                Name = "Test Event 1",
                Description = "Test Event Description",
                Date = DateTime.UtcNow.AddDays(5),
                VenueId = 1,
                TicketsAvailable = 10,
                TotalTickets = 10
            });

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task CreateEventAsync_CreatesEventAndTickets()
        {
            // Arrange
            var context = GetDbContextVenueOnly();
            var service = new EventService(context);
            var request = new EventRequest
            {
                Name = "Concert",
                Description = "Live concert",
                Date = DateTime.UtcNow.AddDays(10),
                VenueId = 1,
                TicketCategories = new List<TicketCategory>
            {
                new() { Type = "VIP", Price = 100, Quantity = 7 },
                new() { Type = "Regular", Price = 50, Quantity = 3 }
            }
            };

            // Act
            var eventResponse = await service.CreateEventAsync(request);

            // Assert
            Assert.NotNull(eventResponse);
            Assert.Equal("Concert", eventResponse.Name);
            Assert.Equal(1, eventResponse.VenueId);
            Assert.True(eventResponse.TicketsAvailable <= eventResponse.Venue.Capacity);
            Assert.Equal(10, eventResponse.TicketsAvailable);


            var fetched = await service.GetEventAsync(eventResponse.Id);

            Assert.NotNull(fetched);
            Assert.Equal(eventResponse.Id, fetched.Id);
        }

        [Fact]
        public async Task UpdateEventAsync_UpdatesEventSuccessfully()
        {
            var context = GetDbContextVenueOnly();
            var service = new EventService(context);

            var initialRequest = new EventRequest
            {
                Name = "Initial",
                Description = "Old desc",
                Date = DateTime.UtcNow.AddDays(1),
                VenueId = 1,
                TicketCategories = new List<TicketCategory>
            {
                new TicketCategory { Type = "Regular", Price = 30, Quantity = 2 }
            }
            };

            var created = await service.CreateEventAsync(initialRequest);

            var updateRequest = new EventRequest
            {
                Name = "Updated",
                Description = "New desc",
                Date = DateTime.UtcNow.AddDays(2),
                VenueId = 1,
                TicketCategories = new List<TicketCategory>
            {
                new TicketCategory { Type = "Regular", Price = 35, Quantity = 4 }
            }
            };

            var success = await service.UpdateEventAsync(created.Id, updateRequest);
            var updated = await service.GetEventAsync(created.Id);

            Assert.True(success);
            Assert.Equal("Updated", updated.Name);
            Assert.Equal(4, updated.TicketsAvailable);
        }
    }
}

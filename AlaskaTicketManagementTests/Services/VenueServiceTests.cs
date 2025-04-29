
using AlaskaTicketManagement.Contracts;
using AlaskaTicketManagement.Data;
using AlaskaTicketManagement.Models;
using AlaskaTicketManagement.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AlaskaTicketManagementTests.Services
{

    public class VenueServiceTests
    {
        private AlaskaConcertDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AlaskaConcertDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AlaskaConcertDbContext(options);
        }

        [Fact]
        public async Task CreateVenueAsync_ShouldAddVenue()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new VenueService(context);

            var request = new VenueRequest
            {
                Name = "Test Venue",
                Location = "Test City",
                Capacity = 500,
                Description = "Test Description"
            };

            // Act
            var result = await service.CreateVenueAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Location, result.Location);
            Assert.Equal(request.Capacity, result.Capacity);
            Assert.Equal(1, context.Venues.Count());
        }

        [Fact]
        public async Task GetAllVenuesAsync_ShouldReturnAllVenues()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Venues.Add(new Venue { Name = "Venue A", Location = "A", Capacity = 100, Description = "Desc A" });
            context.Venues.Add(new Venue { Name = "Venue B", Location = "B", Capacity = 200, Description = "Desc B" });
            await context.SaveChangesAsync();

            var service = new VenueService(context);

            // Act
            var result = await service.GetAllVenuesAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetVenueByIdAsync_ShouldReturnCorrectVenue()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var venue = new Venue { Name = "Single Venue", Location = "Loc", Capacity = 123, Description = "One" };
            context.Venues.Add(venue);
            await context.SaveChangesAsync();

            var service = new VenueService(context);

            // Act
            var result = await service.GetVenueByIdAsync(venue.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(venue.Name, result.Name);
        }

        [Fact]
        public async Task UpdateVenueAsync_ShouldUpdateVenueDetails()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var venue = new Venue { Name = "Old Name", Location = "Old Loc", Capacity = 50, Description = "Old Desc" };
            context.Venues.Add(venue);
            await context.SaveChangesAsync();

            var service = new VenueService(context);
            var updatedRequest = new VenueRequest
            {
                Name = "New Name",
                Location = "New Loc",
                Capacity = 150,
                Description = "New Desc"
            };

            // Act
            var result = await service.UpdateVenueAsync(venue.Id, updatedRequest);

            // Assert
            Assert.True(result);
            var updatedVenue = await context.Venues.FindAsync(venue.Id);
            Assert.Equal("New Name", updatedVenue.Name);
            Assert.Equal(150, updatedVenue.Capacity);
        }

        [Fact]
        public async Task GetVenueByIdAsync_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new VenueService(context);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetVenueByIdAsync(999));
            Assert.Contains("not found", ex.Message);
        }
    }

}
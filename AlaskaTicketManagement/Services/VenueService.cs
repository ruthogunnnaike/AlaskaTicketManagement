using AlaskaTicketManagement.Contracts;
using AlaskaTicketManagement.Data;
using AlaskaTicketManagement.Models;
using AlaskaTicketManagement.Services;
using Microsoft.EntityFrameworkCore;

public class VenueService : IVenueService
{
    private readonly AlaskaConcertDbContext _context;

    public VenueService(AlaskaConcertDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<VenueResponse> CreateVenueAsync(VenueRequest venueRequest)
    {
        var venue = new Venue
        {
            Name = venueRequest.Name,
            Location = venueRequest.Location,
            Capacity = venueRequest.Capacity,
            Description = venueRequest.Description,
        };

        _context.Venues.Add(venue);
        await _context.SaveChangesAsync();
        return await GetVenueByIdAsync(venue.Id) ?? throw new Exception("Venue creation failed");
    }

    public async Task<IEnumerable<VenueResponse>> GetAllVenuesAsync()
    {
        var venues = await _context.Venues.ToListAsync();
        return venues.Select(v => new VenueResponse
        {
           VenueId = v.Id,
           Name = v.Name,
           Location = v.Location,
           Description = v.Description,
           Capacity = v.Capacity
        }).ToList();
    }

    public async Task<VenueResponse> GetVenueByIdAsync(int id)
    {
        var venue = await _context.Venues.FindAsync(id);

        if (venue == null) throw new KeyNotFoundException($"Venue with ID {id} not found.");
        return new VenueResponse
        {
            VenueId = venue.Id,
            Name = venue.Name,
            Location = venue.Location,
            Capacity = venue.Capacity,
            Description = venue.Description,
        };
    }

    public async Task<bool> UpdateVenueAsync(int id, VenueRequest venueRequest)
    {
        var venue = await _context.Venues.FindAsync(id);
        if (venue == null)
        {
            throw new Exception($"Venue with ID {id} not found.");
        }

        venue.Name = venueRequest.Name;
        venue.Location = venueRequest.Location;
        venue.Capacity = venueRequest.Capacity;
        venue.Description = venueRequest.Description;

        await _context.SaveChangesAsync();
        return true;
    }

}

using AlaskaTicketManagement.Contracts;
using AlaskaTicketManagement.Models;

namespace AlaskaTicketManagement.Services
{
    public interface IVenueService
    {
        Task<VenueResponse> CreateVenueAsync(VenueRequest venueRequest);
        Task<IEnumerable<VenueResponse>> GetAllVenuesAsync();
        Task<VenueResponse> GetVenueByIdAsync(int id);
        Task<bool> UpdateVenueAsync(int id, VenueRequest venueRequest);

    }
}

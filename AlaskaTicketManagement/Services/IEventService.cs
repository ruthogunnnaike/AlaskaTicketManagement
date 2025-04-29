using AlaskaTicketManagement.Contracts;
using AlaskaTicketManagement.Models;
using Microsoft.AspNetCore.SignalR;

namespace AlaskaTicketManagement.Services
{
    public interface IEventService
    {
        /// <summary>
        /// Create Event
        /// Requirements met
        /// 1. Create concert events
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<EventResponse> CreateEventAsync(EventRequest request);

        /// <summary>
        /// Get event details.
        /// Requirements met
        /// 1. Get basic event details (date, venue, description)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<EventResponse?> GetEventAsync(int id);

        /// <summary>
        /// Endpoint used to update evet details. 
        /// Requirements met.
        /// 1. Set ticket types and pricing
        /// 2. Manage available capacity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> UpdateEventAsync(int id, EventRequest request);
    }
}

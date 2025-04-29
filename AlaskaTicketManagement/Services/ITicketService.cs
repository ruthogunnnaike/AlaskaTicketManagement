using AlaskaTicketManagement.Contracts;

namespace AlaskaTicketManagement.Services
{
    public interface ITicketService
    {
        /// <summary>
        /// Reserves tickets for a specific event and time window.
        /// </summary>
        /// <param name="request">The reservation request containing event ID, ticket type, quantity, and user email.</param>
        /// <returns>A <see cref="ReservationResponse"/> object containing details of the reservation, including reservation ID, event ID, ticket type, quantity, and expiration time.</returns>
        Task<ReservationResponse> ReserveTicketsAsync(ReservationRequest request);

        /// <summary>
        /// Purchases tickets for a previously made reservation.
        /// </summary>
        /// <param name="request">The purchase request containing the reservation ID.</param>
        /// <returns>A boolean value indicating whether the purchase was successful.</returns>
        Task<bool> PurchaseReservationAsync(TicketPurchaseRequest request);

        /// <summary>
        /// Cancels a previously made ticket reservation.
        /// </summary>
        /// <param name="request">The cancellation request containing the reservation ID and user email.</param>
        /// <returns>A boolean value indicating whether the cancellation was successful.</returns>
        Task<bool> CancelReservationAsync(TicketCancellationRequest request);

        /// <summary>
        /// Retrieves the available ticket categories and their details for a specific event.
        /// </summary>
        /// <param name="eventId">The ID of the event for which ticket availability is being queried.</param>
        /// <returns>A list of <see cref="TicketCategory"/> objects, each containing the ticket type, price, and available quantity.</returns>
        Task<List<TicketCategory>> GetAvailableTicketsAsync(int eventId);
    }
}
using AlaskaTicketManagement.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AlaskaTicketManagement.Services
{
    public interface IPaymentService
    {
        public Task<ActionResult<bool>> ProcessPaymentAsync(TicketPurchaseRequest reservationRequest);

    }
}

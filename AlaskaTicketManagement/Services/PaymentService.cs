using AlaskaTicketManagement.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AlaskaTicketManagement.Services
{
    public class PaymentService: IPaymentService
    {
        /// <summary>
        /// Can assume there is already Payment Processing System in place which you can leverage).
        /// </summary>
        /// <param name="reservationRequest"></param>
        /// <returns></returns>
        public async Task<ActionResult<bool>> ProcessPaymentAsync(TicketPurchaseRequest reservationRequest)
        {
            await Task.Delay(10);
            return new OkObjectResult(true);
        }
    }
}

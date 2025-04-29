using AlaskaTicketManagement.Contracts;
using AlaskaTicketManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AlaskaTicketManagement.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveTickets([FromBody] ReservationRequest request)
        {
            if (request.Quantity <= 0)
                return BadRequest(Problem("Quantity must be greater than 0."));

            try
            {
                var result = await _ticketService.ReserveTicketsAsync(request);
                if (result == null)
                    return BadRequest(Problem("Could not reserve tickets."));

                return Ok(result);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(Problem(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Problem(ex.Message));
            }
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseTickets([FromBody] TicketPurchaseRequest request)
        {
            if (request.ReservationId <= 0)
                return BadRequest(Problem("Invalid reservation ID."));

            try
            {
                var success = await _ticketService.PurchaseReservationAsync(request);
                if (!success)
                    return BadRequest(Problem("Reservation expired or invalid."));

                return Ok(new { message = "Ticket purchased successfully." });
            }
            catch
            {
                return StatusCode(500, Problem("An error occurred while processing your request."));
            }
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelReservation([FromBody] TicketCancellationRequest request)
        {
            if (request.ReservationId <= 0)
                return BadRequest(Problem("Invalid reservation ID."));

            try
            {
                var success = await _ticketService.CancelReservationAsync(request);
                if (!success)
                    return BadRequest(Problem("Unable to cancel reservation. The reservation may not exist or doesn't belong to the user."));

                return Ok(new { message = "Reservation cancelled successfully." });
            }
            catch
            {
                return StatusCode(500, Problem("An error occurred while processing your request."));
            }
        }

        [HttpGet("availability/{eventId}")]
        public async Task<IActionResult> GetAvailability(int eventId)
        {
            if (eventId <= 0)
                return BadRequest(Problem("Invalid event ID."));

            try
            {
                var tickets = await _ticketService.GetAvailableTicketsAsync(eventId);
                if (tickets == null || tickets.Count == 0)
                    return NotFound(Problem("No tickets available for this event."));

                return Ok(new { availableTickets = tickets });
            }
            catch
            {
                return StatusCode(500, Problem("An error occurred while retrieving ticket availability."));
            }
        }

        private ProblemDetails Problem(string detail)
        {
            return new ProblemDetails
            {
                Status = 400,
                Detail = detail,
                Title = "Bad Request"
            };
        }
    }
}

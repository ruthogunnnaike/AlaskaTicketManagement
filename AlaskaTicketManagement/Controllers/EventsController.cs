using Microsoft.AspNetCore.Mvc;
using AlaskaTicketManagement.Contracts;
using AlaskaTicketManagement.Services;
using AlaskaTicketManagement.Models;

namespace AlaskaTicketManagement.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IVenueService _venueService;

        public EventsController(IEventService eventService, IVenueService venueService)
        {
            _eventService = eventService;
            _venueService = venueService;
        }

        /// <summary>Create a new event.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventResponse>> CreateEvent([FromBody] EventRequest request)
        {
            try
            {
                var venue = await _venueService.GetVenueByIdAsync(request.VenueId);
                if (venue == null)
                    return NotFound(new { message = $"Venue with ID {request.VenueId} not found." });

                var validationError = ValidateEventRequest(request, venue);
                if (validationError != null)
                    return validationError;

                var response = await _eventService.CreateEventAsync(request);
                return CreatedAtAction(nameof(GetEvent), new { eventId = response.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the event.", detail = ex.Message });
            }
        }

        /// <summary>Get event details by ID.</summary>
        [HttpGet("{eventId}")]
        [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventResponse>> GetEvent(int eventId)
        {
            try
            {
                var response = await _eventService.GetEventAsync(eventId);
                if (response == null)
                    return NotFound(new { message = $"Event with ID {eventId} not found." });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the event.", detail = ex.Message });
            }
        }

        /// <summary>Update an existing event.</summary>
        [HttpPut("{eventId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEvent(int eventId, [FromBody] EventRequest request)
        {
            try
            {
                var venue = await _venueService.GetVenueByIdAsync(request.VenueId);
                if (venue == null)
                    return BadRequest(new { message = $"Venue with ID {request.VenueId} not found." });

                var validationError = ValidateEventRequest(request, venue);
                if (validationError != null)
                    return validationError;

                var updated = await _eventService.UpdateEventAsync(eventId, request);
                if (!updated)
                    return NotFound(new { message = $"Event with ID {eventId} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the event.", detail = ex.Message });
            }
        }

        /// <summary>Validates an event request against venue constraints.</summary>
        private ActionResult? ValidateEventRequest(EventRequest request, VenueResponse venue)
        {
            int totalRequestedCapacity = request.TicketCategories.Sum(t => t.Quantity);

            if (totalRequestedCapacity > venue.Capacity)
                return BadRequest(new { message = $"Requested ticket quantity ({totalRequestedCapacity}) exceeds venue capacity ({venue.Capacity})." });

            if (request.Date < DateTimeOffset.UtcNow.Date)
                return BadRequest(new { message = "Event date cannot be in the past." });

            return null;
        }
    }
}

using AlaskaTicketManagement.Contracts;
using AlaskaTicketManagement.Models;
using AlaskaTicketManagement.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenuesController(IVenueService venueService)
    {
        _venueService = venueService;
    }

    [HttpPost()]
    public async Task<ActionResult<VenueResponse>> CreateVenue([FromBody] VenueRequest venueRequest)
    {
        if (venueRequest.Capacity <= 0)
        {
            return BadRequest(new { message = "Venue capacity must be greater than 0." });
        }
        try
        {
            var venue = await _venueService.CreateVenueAsync(venueRequest);
            return CreatedAtAction(nameof(GetVenueById), new { venueId = venue.VenueId }, venue);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the venue.", detail = ex.Message });
        }
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<Venue>>> GetAllVenues()
    {
        var venues = await _venueService.GetAllVenuesAsync();
        return Ok(venues);
    }

    [HttpGet("{venueId}")]
    public async Task<ActionResult<Venue>> GetVenueById(int venueId)
    {
        try
        {
            var venue = await _venueService.GetVenueByIdAsync(venueId);
            return Ok(venue);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Venue with ID {venueId} not found." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the venue.", detail = ex.Message });
        }
    }

    [HttpPut("{venueId}")]
    public async Task<IActionResult> UpdateVenue(int venueId, [FromBody] VenueRequest venueRequest)
    {
        if (venueRequest.Capacity <= 0)
        {
            return BadRequest(new { message = "Venue capacity must be greater than 0." });
        }

        try
        {
            await _venueService.UpdateVenueAsync(venueId, venueRequest);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Venue with ID {venueId} not found." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the venue.", detail = ex.Message });
        }
    }
}

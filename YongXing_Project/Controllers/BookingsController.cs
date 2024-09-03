using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using YongXing_Project.Data;
using YongXing_Project.Models;

[Authorize]
[Route("api/[controller]/{action}")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly BookingContext _context;

    private static List<Comment> comments = new List<Comment>();
    public BookingsController(BookingContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        if (User.IsInRole("Admin"))  // Check if the user is an Admin
        {
            return Ok(_context.Bookings.ToList());
        }
        else
        {
            var currentUserName = User.Identity.Name; // Get the currently authenticated user's name
            var userBookings = _context.Bookings
                                       .Where(b => b.BookedBy == currentUserName)
                                       .ToList();
            return Ok(userBookings);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int? id)
    {
        var currentUserName = User.Identity.Name;

        var booking = User.IsInRole("Admin") // Admins can access any booking
            ? _context.Bookings.FirstOrDefault(e => e.BookingID == id)
            : _context.Bookings.FirstOrDefault(e => e.BookingID == id && e.BookedBy == currentUserName);

        if (booking == null)
            return Problem(detail: "Booking with id " + id + " is not found or you do not have permission to view it.", statusCode: 404);

        return Ok(booking);
    }

    [HttpPost]
    public IActionResult Post(Booking booking)
    {
        booking.BookedBy = User.Identity.Name; // Set the booking to the current user
        _context.Bookings.Add(booking);
        _context.SaveChanges();

        return CreatedAtAction("GetAll", new { id = booking.BookingID }, booking);
    }

    [HttpPut]
    public IActionResult Put(int? id, Booking booking)
    {
        var currentUserName = User.Identity.Name;

        var entity = User.IsInRole("Admin") // Admins can update any booking
            ? _context.Bookings.FirstOrDefault(e => e.BookingID == id)
            : _context.Bookings.FirstOrDefault(e => e.BookingID == id && e.BookedBy == currentUserName);

        if (entity == null)
        {
            return Problem(detail: "Booking with id " + id + " is not found or you do not have permission to edit it.", statusCode: 404);
        }

        entity.FacilityDescription = booking.FacilityDescription;
        entity.BookingDateFrom = booking.BookingDateFrom;
        entity.BookingDateTo = booking.BookingDateTo;
        entity.BookedBy = booking.BookedBy;
        entity.BookingStatus = booking.BookingStatus;

        _context.SaveChanges();

        return Ok(entity);
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        if (id == null)
            return BadRequest("Booking ID is required.");

        var currentUserName = User.Identity.Name;

        var entity = User.IsInRole("Admin") // Admins can delete any booking
            ? _context.Bookings.FirstOrDefault(e => e.BookingID == id)
            : _context.Bookings.FirstOrDefault(e => e.BookingID == id && e.BookedBy == currentUserName);

        if (entity == null)
            return NotFound($"Booking with id {id} is not found or you do not have permission to delete it.");

        _context.Bookings.Remove(entity);
        _context.SaveChanges();

        return Ok(entity);
    }

    [HttpGet("{comments}")]
    [AllowAnonymous]
    public IActionResult GetComments([FromQuery] string type)
    {
        if (type == "fixed")
        {
            var fixedComments = new List<string>
            {
                "Great booking experience! Meet my expectation and provides good customer service.",
                "The facility was excellent.",
                "Will definitely use this service again.",
                "We got a great last minute rate with this booking facility and the room was spacious with a comfortable and warm bed."
            };

            return Ok(fixedComments);
        }

        return NotFound("Invalid type specified for comments.");
    }

    [HttpGet("search")]
    public IActionResult Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Search term cannot be empty.");
        }

        var bookings = _context.Bookings.AsQueryable();

        // Apply user-specific filtering
        if (!User.IsInRole("Admin"))
        {
            var currentUserName = User.Identity.Name;
            bookings = bookings.Where(b => b.BookedBy == currentUserName);
        }

        // Apply search filter with case-insensitive comparison
        var filteredBookings = bookings
            .Where(b => b.FacilityDescription.ToLower().Contains(term.ToLower()))
            .ToList();

        return Ok(filteredBookings);
    }

    [HttpGet("sort")]
    public IActionResult GetSortedBookings(string sortBy, string direction)
    {
        var bookings = _context.Bookings.AsQueryable();

        // Check if the user is an admin
        if (!User.IsInRole("Admin"))
        {
            var currentUserName = User.Identity.Name; // Get the currently authenticated user's name
            bookings = bookings.Where(b => b.BookedBy == currentUserName);
        }

        // Apply sorting criteria
        switch (sortBy.ToLower())
        {
            case "facilitydescription":
                bookings = (direction.ToLower() == "asc") ?
                    bookings.OrderBy(b => b.FacilityDescription) :
                    bookings.OrderByDescending(b => b.FacilityDescription);
                break;
            case "bookedby":
                bookings = (direction.ToLower() == "asc") ?
                    bookings.OrderBy(b => b.BookedBy) :
                    bookings.OrderByDescending(b => b.BookedBy);
                break;
            case "bookingdatefrom":
                bookings = (direction.ToLower() == "asc") ?
                    bookings.OrderBy(b => b.BookingDateFrom) :
                    bookings.OrderByDescending(b => b.BookingDateFrom);
                break;
            default:
                return BadRequest("Invalid sort criteria");
        }

        var results = bookings.ToList();

        return Ok(results);
    }
}
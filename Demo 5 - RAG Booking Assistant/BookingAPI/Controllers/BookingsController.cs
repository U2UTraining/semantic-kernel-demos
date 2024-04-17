using DTOs;
using BookingAPI.Infra;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly BookingDbContext _ctx;

    public BookingsController(BookingDbContext ctx)
    {
        _ctx = ctx;
    }

    [HttpPost]
    public async Task<IActionResult> MakeBooking([FromBody]MakeBookingDTO bookingDTO)
    {
        if (bookingDTO is null)
        {
            return BadRequest();
        }
        Customer customer = new Customer
        {
            FirstName = bookingDTO.FirstName,
            LastName = bookingDTO.LastName,
        };
        _ctx.Customers.Add(customer);

        Course course = await _ctx.Courses.SingleOrDefaultAsync(c => c.CourseCode == bookingDTO.CourseCode);
        if(course is null) 
        { 
            return NotFound();
        }
        Booking booking = new Booking
        {
            Customer = customer,
            Course = course,
            CourseDate = bookingDTO.CourseDate
        };
        _ctx.Bookings.Add(booking);
        _ctx.SaveChanges();
        return Created();
    }

    [HttpGet("{courseCode}")]
    public async Task<ActionResult<List<DateTime>>> GetDatesForCourse([FromRoute] string courseCode)
    {
        var dates = _ctx.Courses.SingleOrDefault(c => c.CourseCode == courseCode)?
                                .OrganizationDates.Take(3)
                                .ToList();
        return dates!;
    }
}

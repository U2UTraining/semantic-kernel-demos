namespace Entities;

public class Course
{
    public string CourseCode { get; set; } = default!;
    public List<DateTime> OrganizationDates { get; set; } = default!;
    public List<Booking> Bookings { get; set; } = default!;
}
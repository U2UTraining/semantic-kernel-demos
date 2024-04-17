namespace Entities;

public class Booking
{
    public int Id { get; set; }
    public Course Course { get; set; } = default!;
    public Customer Customer { get; set; } = default!;
    public DateTime CourseDate { get; set; }
}

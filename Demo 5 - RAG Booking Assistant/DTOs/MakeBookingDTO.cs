namespace DTOs;

public class MakeBookingDTO
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string CourseCode { get; set; } = default!;
    public DateTime CourseDate { get; set; }
}

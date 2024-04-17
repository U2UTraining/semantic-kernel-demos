namespace Entities;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public List<Booking> Bookings { get; set; } = default!;
}
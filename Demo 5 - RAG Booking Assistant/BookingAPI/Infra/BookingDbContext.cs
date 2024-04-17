using BookingAPI.Configurations;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingAPI.Infra;

public class BookingDbContext : DbContext
{
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDb;Initial Catalog=DummyBookingDb;Integrated Security=True;Encrypt=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
    }
}

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingAPI.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> booking)
    {
        booking.HasOne(b => b.Course)
               .WithMany(c => c.Bookings);

        booking.HasOne(b => b.Customer)
               .WithMany(c => c.Bookings);
    }
}

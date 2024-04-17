using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingAPI.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> course)
    {
        course.HasKey(c => c.CourseCode);

        course.HasData([new Course { CourseCode = "UNASP", OrganizationDates = [new DateTime(2024, 2, 20), new DateTime(2024, 3, 21), new DateTime(2024, 4, 15)] },
                        new Course { CourseCode = "UCSPR", OrganizationDates = [new DateTime(2024, 2, 21), new DateTime(2024, 3, 22), new DateTime(2024, 4, 17)] },
                        new Course { CourseCode = "UNOOP", OrganizationDates = [new DateTime(2024, 2, 22), new DateTime(2024, 3, 23), new DateTime(2024, 4, 18)] },
                        new Course { CourseCode = "UTSQL", OrganizationDates = [new DateTime(2024, 2, 23), new DateTime(2024, 3, 24), new DateTime(2024, 4, 19)] },
                        new Course { CourseCode = "USJWEB", OrganizationDates = [new DateTime(2024, 2, 24), new DateTime(2024, 3, 25), new DateTime(2024, 4, 20)] },
                        ]);
    }
}

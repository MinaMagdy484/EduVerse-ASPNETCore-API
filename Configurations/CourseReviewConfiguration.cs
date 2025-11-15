using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Configurations
{
    public class CourseReviewConfiguration : IEntityTypeConfiguration<CourseReview>
    {
        public void Configure(EntityTypeBuilder<CourseReview> builder)
        {
            builder.HasKey(cr => cr.Id);

            builder.Property(cr => cr.Rating)
                .IsRequired();

            builder.Property(cr => cr.Comment)
                .HasMaxLength(1000);

            builder.Property(cr => cr.CreatedAt)
                .IsRequired();

            builder.HasOne(cr => cr.Course)
                .WithMany(c => c.CourseReviews)
                .HasForeignKey(cr => cr.CourseID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cr => cr.Student)
                .WithMany(s => s.CourseReviews)
                .HasForeignKey(cr => cr.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(cr => new { cr.CourseID, cr.StudentID }).IsUnique();
        }
    }
}

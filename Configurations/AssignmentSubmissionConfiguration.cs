using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Configurations
{
    public class AssignmentSubmissionConfiguration : IEntityTypeConfiguration<AssignmentSubmission>
    {
        public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
        {
            builder.ToTable("AssignmentSubmissions");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.SubmissionDate)
                .IsRequired();

            builder.Property(s => s.Grade);

            builder.Property(s => s.Feedback)
                .HasMaxLength(1000);

            builder.HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Attachments)
                .WithOne(a => a.AssignmentSubmission)
                .HasForeignKey(a => a.AssignmentSubmissionID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
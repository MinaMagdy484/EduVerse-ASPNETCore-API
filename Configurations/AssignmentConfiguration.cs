using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Configurations
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Deadline)
                .IsRequired();

            // TimeLineItem to Assignment - ONE of the sides in a one-to-one relationship 
            // should NOT cascade to avoid circular references
            builder.HasOne(a => a.TimeLineItem)
                .WithOne(t => t.Assignment)
                .HasForeignKey<Assignment>(a => a.TimeLineItemID)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment to Submissions - Allow cascade
            builder.HasMany(a => a.Submissions)
                .WithOne(s => s.Assignment)
                .HasForeignKey(s => s.AssignmentID)
                .OnDelete(DeleteBehavior.Cascade);

            // Assignment to AllowedExtensions - Allow cascade
            builder.HasMany(a => a.AllowedExtensions)
                .WithOne(e => e.Assignment)
                .HasForeignKey(e => e.AssignmentID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
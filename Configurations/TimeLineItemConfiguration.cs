using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Configurations
{
    public class TimeLineItemConfiguration : IEntityTypeConfiguration<TimeLineItem>
    {
        public void Configure(EntityTypeBuilder<TimeLineItem> builder)
        {
            builder.ToTable("TimeLineItems");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(t => t.Content)
                .HasMaxLength(5000);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            // Course to TimeLineItem - Use Restrict to avoid cascade loops
            builder.HasOne(t => t.Course)
                .WithMany()
                .HasForeignKey(t => t.CourseID)
                .OnDelete(DeleteBehavior.Restrict);

            // User to TimeLineItem - Use Restrict to avoid cascade loops
            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // TimeLineItem to Attachments - Allow cascade
            builder.HasMany(t => t.Attachments)
                .WithOne(a => a.TimeLineItem)
                .HasForeignKey(a => a.TimeLineItemID)
                .OnDelete(DeleteBehavior.Cascade);

            // TimeLineItem to Comments - Allow cascade
            builder.HasMany(t => t.Comments)
                .WithOne(c => c.TimeLineItem)
                .HasForeignKey(c => c.TimeLineItemID)
                .OnDelete(DeleteBehavior.Cascade);

            // TimeLineItem to Post - Allow cascade from parent
            builder.HasOne(t => t.Post)
                .WithOne(p => p.TimeLineItem)
                .HasForeignKey<Post>(p => p.TimeLineItemID)
                .OnDelete(DeleteBehavior.Cascade);

            // TimeLineItem to Assignment - Allow cascade from parent
            builder.HasOne(t => t.Assignment)
                .WithOne(a => a.TimeLineItem)
                .HasForeignKey<Assignment>(a => a.TimeLineItemID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Configurations
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ToTable("Attachments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(a => a.FileType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.FileSize)
                .IsRequired();

            builder.Property(a => a.UploadDate)
                .IsRequired();

            builder.HasOne(a => a.TimeLineItem)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TimeLineItemID)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(a => a.AssignmentSubmission)
                .WithMany(s => s.Attachments)
                .HasForeignKey(a => a.AssignmentSubmissionID)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
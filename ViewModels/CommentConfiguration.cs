using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(c => c.Time)
                .IsRequired();

            builder.HasOne(c => c.TimeLineItem)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TimeLineItemID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.CommentedBy)
                .WithMany()
                .HasForeignKey(c => c.CommentedByID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
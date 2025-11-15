using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Content)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.HasOne(p => p.TimeLineItem)
                .WithOne(t => t.Post)
                .HasForeignKey<Post>(p => p.TimeLineItemID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
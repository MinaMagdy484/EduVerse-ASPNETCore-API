using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Configurations
{
    public class AssignmentAllowedExtensionConfiguration : IEntityTypeConfiguration<AssignmentAllowedExtension>
    {
        public void Configure(EntityTypeBuilder<AssignmentAllowedExtension> builder)
        {
            builder.ToTable("AssignmentAllowedExtensions");
            
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Extension)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(e => e.Assignment)
                .WithMany(a => a.AllowedExtensions)
                .HasForeignKey(e => e.AssignmentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
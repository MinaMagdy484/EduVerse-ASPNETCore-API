using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Configurations
{
    public class ExamStudentConfiguration : IEntityTypeConfiguration<ExamStudent>
    {
        public void Configure(EntityTypeBuilder<ExamStudent> builder)
        {
            builder.ToTable("ExamStudent");  // Change to match existing table name

            builder.HasOne(es => es.Student)
                .WithMany(s => s.ExamStudents)
                .HasForeignKey(es => es.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(es => es.Exam)
                .WithMany(e => e.ExamStudents)
                .HasForeignKey(es => es.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(es => es.ExamAnswers)
                .WithOne(ea => ea.ExamStudent)
                .HasForeignKey(ea => ea.ExamStudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
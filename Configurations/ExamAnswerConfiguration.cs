using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Configurations
{
    public class ExamAnswerConfiguration : IEntityTypeConfiguration<ExamAnswer>
    {
        public void Configure(EntityTypeBuilder<ExamAnswer> builder)
        {
            builder.ToTable("ExamAnswer");  // Change to match existing table name

            builder.HasOne(ea => ea.ExamStudent)
                .WithMany(es => es.ExamAnswers)
                .HasForeignKey(ea => ea.ExamStudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ea => ea.ExamQuestion)
                .WithMany()
                .HasForeignKey(ea => ea.ExamQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ea => ea.Choice)
                .WithMany()
                .HasForeignKey(ea => ea.ChoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
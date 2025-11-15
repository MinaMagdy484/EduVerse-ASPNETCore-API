using ExaminationSystem.Configurations;
using ExaminationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Data
{
    public class Context : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }

            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            modelBuilder.Entity<User>().ToTable("Users")
                .HasDiscriminator<string>("UserType")
                .HasValue<Student>("Student")
                .HasValue<Instructor>("Instructor");

            modelBuilder.ApplyConfiguration(new ChoiceConfiguration());
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
            modelBuilder.ApplyConfiguration(new CourseInstructorConfiguration());
            modelBuilder.ApplyConfiguration(new CourseStudentConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new ExamConfiguration());
            modelBuilder.ApplyConfiguration(new ExamQuestionConfiguration());
            modelBuilder.ApplyConfiguration(new InstructorConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new ExamAnswerConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentAllowedExtensionConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentSubmissionConfiguration());
            modelBuilder.ApplyConfiguration(new AttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new TimeLineItemConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());  
            modelBuilder.ApplyConfiguration(new PostConfiguration());   
            modelBuilder.ApplyConfiguration(new ExamStudentConfiguration());  
            modelBuilder.ApplyConfiguration(new CourseReviewConfiguration());

        }

        public DbSet<ExamStudent> ExamStudents { get; set; }
        public DbSet<ExamAnswer> ExamAnswers { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseInstructor> CourseInstructors { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Student> Students { get; set; }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentAllowedExtension> AssignmentAllowedExtensions { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<TimeLineItem> TimeLineItems { get; set; }
        public DbSet<Comment> Comments { get; set; }     
        public DbSet<Post> Posts { get; set; }     
        public DbSet<CourseReview> CourseReviews { get; set; }

    }
}
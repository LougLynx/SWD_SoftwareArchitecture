using Microsoft.EntityFrameworkCore;

namespace SWD_SoftwareArchitecture.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet Properties
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Certification> Certifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Message entity - Self-referencing relationships
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure cascade delete behaviors
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Modules)
                .WithOne(m => m.Course)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Module>()
                .HasMany(m => m.Lessons)
                .WithOne(l => l.Module)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ForumThread>()
                .HasMany(ft => ft.ForumPosts)
                .WithOne(fp => fp.ForumThread)
                .HasForeignKey(fp => fp.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);

            // Fix cascade delete conflicts
            modelBuilder.Entity<ForumThread>()
                .HasOne(ft => ft.User)
                .WithMany(u => u.ForumThreads)
                .HasForeignKey(ft => ft.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ForumPost>()
                .HasOne(fp => fp.User)
                .WithMany(u => u.ForumPosts)
                .HasForeignKey(fp => fp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuizAttempt>()
                .HasOne(qa => qa.User)
                .WithMany(u => u.QuizAttempts)
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignmentSubmission>()
                .HasOne(asub => asub.User)
                .WithMany(u => u.AssignmentSubmissions)
                .HasForeignKey(asub => asub.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Lesson)
                .WithMany(l => l.Progresses)
                .HasForeignKey(p => p.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Set default values
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Course>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Assignment>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ForumThread>()
                .Property(ft => ft.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ForumPost>()
                .Property(fp => fp.PostDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Message>()
                .Property(m => m.SendTime)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Notification>()
                .Property(n => n.SentDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Notification>()
                .Property(n => n.IsRead)
                .HasDefaultValue(false);

            modelBuilder.Entity<Report>()
                .Property(r => r.GeneratedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Certification>()
                .Property(c => c.IssueDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<QuizAttempt>()
                .Property(qa => qa.StartTime)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<AssignmentSubmission>()
                .Property(a => a.SubmittedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}


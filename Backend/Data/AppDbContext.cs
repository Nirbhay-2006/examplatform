using Microsoft.EntityFrameworkCore;
using ExamNest.Models;

namespace ExamNest.Data
{
    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGoogleAuth> UserGoogleAuths { get; set; }
        public DbSet<EmailOtp> EmailOtps { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseMedia> CourseMedias { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<CourseSubscription> CourseSubscriptions { get; set; }
        public DbSet<CourseExam> CourseExams { get; set; }
        public DbSet<ExamSession> ExamSessions { get; set; }
        public DbSet<ViolationLog> ViolationLogs { get; set; }
        public DbSet<MonitoringLog> MonitoringLogs { get; set; }
        public DbSet<ExamSubmission> ExamSubmissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Roles
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email);
                //.IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.IsActive)
                .HasDefaultValue(false);

            modelBuilder.Entity<User>()
                .Property(u => u.FailedLoginAttempts)
                .HasDefaultValue(0);

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // UserGoogleAuth
            modelBuilder.Entity<UserGoogleAuth>()
                .HasIndex(g => g.GoogleSub)
                .IsUnique();

            modelBuilder.Entity<UserGoogleAuth>()
                .HasIndex(g => g.GoogleEmail)
                .IsUnique();

            modelBuilder.Entity<UserGoogleAuth>()
                .Property(g => g.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // EmailOtp
            modelBuilder.Entity<EmailOtp>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<EmailOtp>()
                .Property(e => e.IsUsed)
                .HasDefaultValue(false);

            // ExamSessions
            modelBuilder.Entity<ExamSession>()
                .Property(s => s.StartTime)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<ExamSession>()
                .Property(s => s.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<ExamSession>()
                .HasIndex(s => s.Status);

            modelBuilder.Entity<ExamSession>()
                .HasIndex(s => new { s.ExamId, s.UserId, s.Status });

            modelBuilder.Entity<ExamSession>()
                .HasIndex(s => new { s.UserId, s.ExamId })
                .HasFilter("[status] = 'Active'")
                .IsUnique();

            // ViolationLogs
            modelBuilder.Entity<ViolationLog>()
                .Property(v => v.ViolationType)
                .HasConversion<string>()
                .HasMaxLength(40);

            modelBuilder.Entity<ViolationLog>()
                .Property(v => v.Timestamp)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<ViolationLog>()
                .HasIndex(v => new { v.SessionId, v.Timestamp });

            // MonitoringLogs
            modelBuilder.Entity<MonitoringLog>()
                .Property(m => m.EventTime)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<MonitoringLog>()
                .HasIndex(m => new { m.SessionId, m.EventTime });

            // ExamSubmissions
            modelBuilder.Entity<ExamSubmission>()
                .Property(s => s.SubmittedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<ExamSubmission>()
                .Property(s => s.Mode)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<ExamSubmission>()
                .HasIndex(s => s.SessionId)
                .IsUnique();

            // Course and CourseMedia
            modelBuilder.Entity<Course>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Course>()
                .HasMany(c => c.CourseMedias)
                .WithOne(cm => cm.Course)
                .HasForeignKey(cm => cm.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseSubscription>()
                .HasIndex(s => new { s.CourseId, s.StudentId })
                .IsUnique();

            modelBuilder.Entity<CourseSubscription>()
                .Property(s => s.SubscribedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<CourseSubscription>()
                .Property(s => s.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<CourseSubscription>()
                .HasOne(s => s.Course)
                .WithMany()
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseSubscription>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseExam>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<CourseExam>()
                .HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<UserGoogleAuth>()
                .HasOne(g => g.User)
                .WithMany()
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailOtp>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamSession>()
                .HasMany(s => s.ViolationLogs)
                .WithOne(v => v.Session)
                .HasForeignKey(v => v.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamSession>()
                .HasMany(s => s.MonitoringLogs)
                .WithOne(m => m.Session)
                .HasForeignKey(m => m.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamSession>()
                .HasOne(s => s.Submission)
                .WithOne(sub => sub.Session)
                .HasForeignKey<ExamSubmission>(sub => sub.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Role { RoleId = 2, RoleName = "Teacher", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Role { RoleId = 3, RoleName = "Student", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

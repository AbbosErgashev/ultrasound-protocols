using Microsoft.EntityFrameworkCore;
using UltrasoundProtocol.Domain.Entities;

namespace UltrasoundProtocol.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UltrasoundExam> UltrasoundExams => Set<UltrasoundExam>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
    public DbSet<ServiceItem> ServiceItems => Set<ServiceItem>();
    public DbSet<DoctorProfile> DoctorProfiles => Set<DoctorProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.PhoneNumber).IsUnique();
            entity.Property(p => p.CreatedAt)
                    .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        });

        modelBuilder.Entity<UltrasoundExam>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DoctorUsername).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BodyPart).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Findings).IsRequired();
            entity.Property(e => e.Conclusion).IsRequired();
            entity.HasOne(e => e.Patient)
                  .WithMany(u => u.UltrasoundProtocols)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Diagnosis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IcdCode).HasMaxLength(20);
            entity.Property(e => e.DiagnosisName).IsRequired().HasMaxLength(300);
            entity.Property(e => e.DoctorUsername).HasMaxLength(100);
            entity.HasOne(e => e.Patient)
                  .WithMany(u => u.Diagnoses)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Protocol)
                  .WithMany(p => p.Diagnoses)
                  .HasForeignKey(e => e.ProtocolId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GeneratedBy).HasMaxLength(100);
            entity.HasOne(e => e.Protocol)
                  .WithOne(p => p.Report)
                  .HasForeignKey<Report>(e => e.ProtocolId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DoctorUsername).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Patient)
                  .WithMany(u => u.Appointments)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RecipientUsername).HasMaxLength(100);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Notifications)
                  .HasForeignKey(e => e.UserId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityId).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
        });

        modelBuilder.Entity<NewsArticle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Summary).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.VideoUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<ServiceItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<DoctorProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Specialty).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Bio).HasMaxLength(2000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(30);
        });
    }
}

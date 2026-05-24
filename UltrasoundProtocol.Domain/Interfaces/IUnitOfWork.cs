using UltrasoundProtocol.Domain.Entities;

namespace UltrasoundProtocol.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<UltrasoundExam> UltrasoundExams { get; }
    IRepository<Diagnosis> Diagnoses { get; }
    IRepository<Report> Reports { get; }
    IRepository<BreastUltrasoundProtocol> BreastUltrasoundProtocols { get; }
    IRepository<Appointment> Appointments { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<NewsArticle> NewsArticles { get; }
    IRepository<ServiceItem> ServiceItems { get; }
    IRepository<DoctorProfile> DoctorProfiles { get; }

    Task<int> SaveChangesAsync();
}

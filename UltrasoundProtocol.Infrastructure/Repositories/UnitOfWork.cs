using UltrasoundProtocol.Domain.Entities;
using UltrasoundProtocol.Domain.Interfaces;
using UltrasoundProtocol.Infrastructure.Data;

namespace UltrasoundProtocol.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRepository<User> Users { get; }
    public IRepository<UltrasoundExam> UltrasoundExams { get; }
    public IRepository<Diagnosis> Diagnoses { get; }
    public IRepository<Report> Reports { get; }
    public IRepository<BreastUltrasoundProtocol> BreastUltrasoundProtocols { get; }
    public IRepository<Appointment> Appointments { get; }
    public IRepository<Notification> Notifications { get; }
    public IRepository<AuditLog> AuditLogs { get; }
    public IRepository<NewsArticle> NewsArticles { get; }
    public IRepository<ServiceItem> ServiceItems { get; }
    public IRepository<DoctorProfile> DoctorProfiles { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new GenericRepository<User>(context);
        UltrasoundExams = new GenericRepository<UltrasoundExam>(context);
        Diagnoses = new GenericRepository<Diagnosis>(context);
        Reports = new GenericRepository<Report>(context);
        BreastUltrasoundProtocols = new GenericRepository<BreastUltrasoundProtocol>(context);
        Appointments = new GenericRepository<Appointment>(context);
        Notifications = new GenericRepository<Notification>(context);
        AuditLogs = new GenericRepository<AuditLog>(context);
        NewsArticles = new GenericRepository<NewsArticle>(context);
        ServiceItems = new GenericRepository<ServiceItem>(context);
        DoctorProfiles = new GenericRepository<DoctorProfile>(context);
    }

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Dispose() =>
        _context.Dispose();
}

using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UltrasoundProtocol.Application.Mappings;
using UltrasoundProtocol.Application.Services.Appointment;
using UltrasoundProtocol.Application.Services.Audit;
using UltrasoundProtocol.Application.Services.Auth;
using UltrasoundProtocol.Application.Services.BreastProtocol;
using UltrasoundProtocol.Application.Services.Content;
using UltrasoundProtocol.Application.Services.Email;
using UltrasoundProtocol.Application.Services.Excel;
using UltrasoundProtocol.Application.Services.FileStorage;
using UltrasoundProtocol.Application.Services.Notification;
using UltrasoundProtocol.Application.Services.Patient;
using UltrasoundProtocol.Application.Services.Pdf;
using UltrasoundProtocol.Application.Services.Protocol;
using UltrasoundProtocol.Application.Services.Report;
using UltrasoundProtocol.Application.Services.Statistics;
using UltrasoundProtocol.Application.Services.Templates;
using UltrasoundProtocol.Application.Settings;

namespace UltrasoundProtocol.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Settings
        services.Configure<StaticUserSettings>(
            configuration.GetSection("StaticUsers"));
        services.Configure<EmailSettings>(
            configuration.GetSection("EmailSettings"));
        // AutoMapper
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<MappingProfile>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IProtocolService, ProtocolService>();
        services.AddScoped<IBreastProtocolService, BreastProtocolService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddSingleton<IFileStorageService, LocalFileStorageService>();
        services.AddSingleton<ITemplateService, TemplateService>();
        services.AddScoped<IContentService, ContentService>();

        return services;
    }
}

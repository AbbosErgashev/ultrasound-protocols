using AutoMapper;
using UltrasoundProtocol.Application.DTOs.Appointment;
using UltrasoundProtocol.Application.DTOs.Content;
using UltrasoundProtocol.Application.DTOs.Diagnosis;
using UltrasoundProtocol.Application.DTOs.Notification;
using UltrasoundProtocol.Application.DTOs.Patient;
using UltrasoundProtocol.Application.DTOs.Protocol;
using UltrasoundProtocol.Application.DTOs.Report;
using UltrasoundProtocol.Domain.Entities;

namespace UltrasoundProtocol.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, PatientDto>();
        CreateMap<PatientCreateDto, User>();

        CreateMap<UltrasoundExam, ProtocolDto>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient == null ? string.Empty : s.Patient.FullName));
        CreateMap<ProtocolCreateDto, UltrasoundExam>();

        CreateMap<Diagnosis, DiagnosisDto>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.FullName));
        CreateMap<DiagnosisCreateDto, Diagnosis>();

        CreateMap<Report, ReportDto>();
        CreateMap<ReportCreateDto, Report>();

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.FullName));
        CreateMap<AppointmentCreateDto, Appointment>();

        CreateMap<Notification, NotificationDto>();

        // Content
        CreateMap<NewsArticle, NewsArticleDto>();
        CreateMap<NewsCreateDto, NewsArticle>();

        CreateMap<ServiceItem, ServiceItemDto>();
        CreateMap<ServiceCreateDto, ServiceItem>();

        CreateMap<DoctorProfile, DoctorProfileDto>();
        CreateMap<DoctorProfileCreateDto, DoctorProfile>();
    }
}

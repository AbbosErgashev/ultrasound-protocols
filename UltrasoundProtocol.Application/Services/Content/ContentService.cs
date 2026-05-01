using AutoMapper;
using Microsoft.Extensions.Logging;
using UltrasoundProtocol.Application.DTOs.Content;
using UltrasoundProtocol.Domain.Entities;
using UltrasoundProtocol.Domain.Interfaces;

namespace UltrasoundProtocol.Application.Services.Content;

public interface IContentService
{
    // News
    Task<IEnumerable<NewsArticleDto>> GetAllNewsAsync();
    Task<IEnumerable<NewsArticleDto>> GetPublishedNewsAsync();
    Task<NewsArticleDto?> GetNewsByIdAsync(Guid id);
    Task<NewsArticleDto> CreateNewsAsync(NewsCreateDto dto, string author);
    Task<bool> UpdateNewsAsync(Guid id, NewsCreateDto dto);
    Task<bool> TogglePublishNewsAsync(Guid id);
    Task<bool> DeleteNewsAsync(Guid id);

    // Services
    Task<IEnumerable<ServiceItemDto>> GetAllServicesAsync();
    Task<IEnumerable<ServiceItemDto>> GetActiveServicesAsync();
    Task<ServiceItemDto> CreateServiceAsync(ServiceCreateDto dto);
    Task<bool> UpdateServiceAsync(Guid id, ServiceCreateDto dto);
    Task<bool> ToggleServiceAsync(Guid id);
    Task<bool> DeleteServiceAsync(Guid id);

    // Doctors
    Task<IEnumerable<DoctorProfileDto>> GetAllDoctorsAsync();
    Task<IEnumerable<DoctorProfileDto>> GetActiveDoctorsAsync();
    Task<DoctorProfileDto> CreateDoctorAsync(DoctorProfileCreateDto dto);
    Task<bool> UpdateDoctorAsync(Guid id, DoctorProfileCreateDto dto);
    Task<bool> ToggleDoctorAsync(Guid id);
    Task<bool> DeleteDoctorAsync(Guid id);
}

public class ContentService : IContentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ContentService> _logger;

    public ContentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ContentService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // ===== NEWS =====

    public async Task<IEnumerable<NewsArticleDto>> GetAllNewsAsync()
    {
        _logger.LogDebug("Barcha yangiliklar so'raldi");
        var items = await _unitOfWork.NewsArticles.GetAllAsync();
        return _mapper.Map<IEnumerable<NewsArticleDto>>(items);
    }

    public async Task<IEnumerable<NewsArticleDto>> GetPublishedNewsAsync()
    {
        var items = await _unitOfWork.NewsArticles.FindAsync(n => n.IsPublished);
        return _mapper.Map<IEnumerable<NewsArticleDto>>(items);
    }

    public async Task<NewsArticleDto?> GetNewsByIdAsync(Guid id)
    {
        var item = await _unitOfWork.NewsArticles.GetByIdAsync(id);
        return item is null ? null : _mapper.Map<NewsArticleDto>(item);
    }

    public async Task<NewsArticleDto> CreateNewsAsync(NewsCreateDto dto, string author)
    {
        _logger.LogInformation("Yangilik yaratilmoqda: {Title}, Muallif: {Author}", dto.Title, author);
        var entity = _mapper.Map<NewsArticle>(dto);
        entity.Author = author;
        await _unitOfWork.NewsArticles.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Yangilik yaratildi: {NewsId}", entity.Id);
        return _mapper.Map<NewsArticleDto>(entity);
    }

    public async Task<bool> UpdateNewsAsync(Guid id, NewsCreateDto dto)
    {
        _logger.LogInformation("Yangilik yangilanmoqda: {NewsId}", id);
        var entity = await _unitOfWork.NewsArticles.GetByIdAsync(id);
        if (entity is null) { _logger.LogWarning("Yangilik topilmadi: {NewsId}", id); return false; }
        entity.Title = dto.Title;
        entity.Summary = dto.Summary;
        entity.Content = dto.Content;
        entity.ImageUrl = dto.ImageUrl;
        entity.VideoUrl = dto.VideoUrl;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.NewsArticles.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TogglePublishNewsAsync(Guid id)
    {
        var entity = await _unitOfWork.NewsArticles.GetByIdAsync(id);
        if (entity is null) return false;
        entity.IsPublished = !entity.IsPublished;
        entity.PublishedAt = entity.IsPublished ? DateTime.UtcNow : null;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.NewsArticles.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteNewsAsync(Guid id)
    {
        var entity = await _unitOfWork.NewsArticles.GetByIdAsync(id);
        if (entity is null) return false;
        _unitOfWork.NewsArticles.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // ===== SERVICES =====

    public async Task<IEnumerable<ServiceItemDto>> GetAllServicesAsync()
    {
        var items = await _unitOfWork.ServiceItems.GetAllAsync();
        return _mapper.Map<IEnumerable<ServiceItemDto>>(items);
    }

    public async Task<IEnumerable<ServiceItemDto>> GetActiveServicesAsync()
    {
        var items = await _unitOfWork.ServiceItems.FindAsync(s => s.IsActive);
        return _mapper.Map<IEnumerable<ServiceItemDto>>(items);
    }

    public async Task<ServiceItemDto> CreateServiceAsync(ServiceCreateDto dto)
    {
        var entity = _mapper.Map<ServiceItem>(dto);
        await _unitOfWork.ServiceItems.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ServiceItemDto>(entity);
    }

    public async Task<bool> UpdateServiceAsync(Guid id, ServiceCreateDto dto)
    {
        var entity = await _unitOfWork.ServiceItems.GetByIdAsync(id);
        if (entity is null) return false;
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Icon = dto.Icon;
        entity.Price = dto.Price;
        entity.SortOrder = dto.SortOrder;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ServiceItems.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleServiceAsync(Guid id)
    {
        var entity = await _unitOfWork.ServiceItems.GetByIdAsync(id);
        if (entity is null) return false;
        entity.IsActive = !entity.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ServiceItems.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteServiceAsync(Guid id)
    {
        var entity = await _unitOfWork.ServiceItems.GetByIdAsync(id);
        if (entity is null) return false;
        _unitOfWork.ServiceItems.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // ===== DOCTORS =====

    public async Task<IEnumerable<DoctorProfileDto>> GetAllDoctorsAsync()
    {
        var items = await _unitOfWork.DoctorProfiles.GetAllAsync();
        return _mapper.Map<IEnumerable<DoctorProfileDto>>(items);
    }

    public async Task<IEnumerable<DoctorProfileDto>> GetActiveDoctorsAsync()
    {
        var items = await _unitOfWork.DoctorProfiles.FindAsync(d => d.IsActive);
        return _mapper.Map<IEnumerable<DoctorProfileDto>>(items);
    }

    public async Task<DoctorProfileDto> CreateDoctorAsync(DoctorProfileCreateDto dto)
    {
        var entity = _mapper.Map<DoctorProfile>(dto);
        await _unitOfWork.DoctorProfiles.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<DoctorProfileDto>(entity);
    }

    public async Task<bool> UpdateDoctorAsync(Guid id, DoctorProfileCreateDto dto)
    {
        var entity = await _unitOfWork.DoctorProfiles.GetByIdAsync(id);
        if (entity is null) return false;
        entity.FullName = dto.FullName;
        entity.Specialty = dto.Specialty;
        entity.Bio = dto.Bio;
        entity.ImageUrl = dto.ImageUrl;
        entity.Phone = dto.Phone;
        entity.ExperienceYears = dto.ExperienceYears;
        entity.SortOrder = dto.SortOrder;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.DoctorProfiles.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleDoctorAsync(Guid id)
    {
        var entity = await _unitOfWork.DoctorProfiles.GetByIdAsync(id);
        if (entity is null) return false;
        entity.IsActive = !entity.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.DoctorProfiles.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDoctorAsync(Guid id)
    {
        var entity = await _unitOfWork.DoctorProfiles.GetByIdAsync(id);
        if (entity is null) return false;
        _unitOfWork.DoctorProfiles.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}

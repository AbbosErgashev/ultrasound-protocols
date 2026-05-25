using AutoMapper;
using Microsoft.Extensions.Logging;
using UltrasoundProtocol.Application.DTOs.Patient;
using UltrasoundProtocol.Domain.Entities;
using UltrasoundProtocol.Domain.Enums;
using UltrasoundProtocol.Domain.Interfaces;

namespace UltrasoundProtocol.Application.Services.Patient;

public class PatientService : IPatientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PatientService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
        _logger.LogDebug("Barcha bemorlar ro'yxati so'raldi");
        var users = await _unitOfWork.Users.FindAsync(u => u.Role == UserRole.User);
        var orderedUsers = users.OrderByDescending(u => u.CreatedDate);
        var result = _mapper.Map<IEnumerable<PatientDto>>(orderedUsers);
        _logger.LogInformation("Bemorlar ro'yxati qaytarildi: {Count} ta", result.Count());
        return result;
    }

    public async Task<PatientDto?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Bemor so'raldi: {PatientId}", id);
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
        {
            _logger.LogWarning("Bemor topilmadi: {PatientId}", id);
            return null;
        }
        return _mapper.Map<PatientDto>(user);
    }

    public async Task<PatientDto> CreateAsync(PatientCreateDto dto, string doctorUsername)
    {
        _logger.LogInformation("Yangi bemor yaratilmoqda: {FullName}, Username: {Username}, Shifokor: {Doctor}",
            dto.FullName, dto.Username, doctorUsername);

        var user = _mapper.Map<User>(dto);
        user.Username = dto.Username;
        user.PasswordHash = dto.Password;
        user.Role = UserRole.User;
        user.IsActive = true;
        user.CreatedDate = DateTime.UtcNow;

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Bemor yaratildi: {FullName}, Username: {Username} (ID: {PatientId})",
            user.FullName, user.Username, user.Id);
        return _mapper.Map<PatientDto>(user);
    }

    public async Task<PatientDto?> UpdateAsync(Guid id, PatientCreateDto dto)
    {
        _logger.LogInformation("Bemor yangilanmoqda: {PatientId}", id);

        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
        {
            _logger.LogWarning("Yangilanish uchun bemor topilmadi: {PatientId}", id);
            return null;
        }

        user.FullName = dto.FullName;
        user.DateOfBirth = dto.DateOfBirth;
        user.Gender = dto.Gender;
        user.PhoneNumber = dto.PhoneNumber;
        user.Email = dto.Email;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Bemor yangilandi: {FullName} (ID: {PatientId})", user.FullName, user.Id);
        return _mapper.Map<PatientDto>(user);
    }

    public async Task<bool> ChangeCredentialsAsync(Guid id, string newUsername, string newPassword)
    {
        _logger.LogInformation("Bemor login ma'lumotlari o'zgartirilmoqda: {PatientId}", id);

        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
        {
            _logger.LogWarning("Credential o'zgartirish uchun bemor topilmadi: {PatientId}", id);
            return false;
        }

        if (!string.IsNullOrEmpty(newUsername))
            user.Username = newUsername;

        if (!string.IsNullOrEmpty(newPassword))
            user.PasswordHash = newPassword;

        user.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Bemor login ma'lumotlari o'zgartirildi: {Username} (ID: {PatientId})",
            user.Username, user.Id);
        return true;
    }
}

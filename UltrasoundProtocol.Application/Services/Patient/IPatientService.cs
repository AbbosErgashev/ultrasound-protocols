using UltrasoundProtocol.Application.DTOs.Patient;

namespace UltrasoundProtocol.Application.Services.Patient;

public interface IPatientService
{
    Task<IEnumerable<PatientDto>> GetAllAsync();
    Task<PatientDto?> GetByIdAsync(Guid id);
    Task<PatientDto> CreateAsync(PatientCreateDto dto, string doctorUsername);
    Task<PatientDto?> UpdateAsync(Guid id, PatientCreateDto dto);
    Task<bool> ChangeCredentialsAsync(Guid id, string newUsername, string newPassword);
}

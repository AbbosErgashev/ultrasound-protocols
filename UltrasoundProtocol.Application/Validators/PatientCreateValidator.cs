using FluentValidation;
using UltrasoundProtocol.Application.DTOs.Patient;

namespace UltrasoundProtocol.Application.Validators;

public class PatientCreateValidator : AbstractValidator<PatientCreateDto>
{
    public PatientCreateValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("To'liq ism kiritilishi shart")
            .MaximumLength(200).WithMessage("Ism 200 ta belgidan oshmasligi kerak");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Now).WithMessage("Tug'ilgan sana noto'g'ri");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Jins ko'rsatilishi shart");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon raqam kiritilishi shart")
            .Matches(@"^\+998\d{9}$").WithMessage("Telefon formati: +998XXXXXXXXX");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parol kiritilishi shart")
            .MinimumLength(8).WithMessage("Parol kamida 8 ta belgidan iborat bo'lishi kerak");
    }
}

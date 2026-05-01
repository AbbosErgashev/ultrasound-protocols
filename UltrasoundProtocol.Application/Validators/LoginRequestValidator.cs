using FluentValidation;
using UltrasoundProtocol.Application.DTOs.Auth;

namespace UltrasoundProtocol.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Foydalanuvchi nomi kiritilishi shart")
            .MinimumLength(3).WithMessage("Kamida 3 ta belgi bo'lishi kerak");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parol kiritilishi shart")
            .MinimumLength(6).WithMessage("Parol kamida 6 ta belgidan iborat bo'lishi kerak");
    }
}

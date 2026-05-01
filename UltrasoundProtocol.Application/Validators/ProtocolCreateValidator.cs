using FluentValidation;
using UltrasoundProtocol.Application.DTOs.Protocol;

namespace UltrasoundProtocol.Application.Validators;

public class ProtocolCreateValidator : AbstractValidator<ProtocolCreateDto>
{
    public ProtocolCreateValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Bemor tanlanishi shart");

        RuleFor(x => x.BodyPart)
            .NotEmpty().WithMessage("Tekshirilgan organ ko'rsatilishi shart")
            .MaximumLength(100).WithMessage("100 ta belgidan oshmasligi kerak");

        RuleFor(x => x.ExamDate)
            .NotEmpty().WithMessage("Tekshiruv sanasi kiritilishi shart");

        RuleFor(x => x.Findings)
            .NotEmpty().WithMessage("Topilmalar kiritilishi shart");

        RuleFor(x => x.Conclusion)
            .NotEmpty().WithMessage("Xulosa kiritilishi shart");
    }
}

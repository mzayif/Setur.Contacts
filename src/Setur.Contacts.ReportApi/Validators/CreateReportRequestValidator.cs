using FluentValidation;
using Setur.Contacts.ReportApi.DTOs.Requests;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.ReportApi.Validators;

public class CreateReportRequestValidator : AbstractValidator<CreateReportRequest>
{
    public CreateReportRequestValidator()
    {
        RuleFor(x => x.ReportType)
            .IsInEnum().WithMessage("Geçersiz rapor türü");

        RuleFor(x => x.Parameters)
            .NotNull().WithMessage("Parametreler boş olamaz");
    }
}

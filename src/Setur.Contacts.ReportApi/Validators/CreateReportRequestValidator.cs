using FluentValidation;
using Setur.Contacts.ReportApi.DTOs.Requests;

namespace Setur.Contacts.ReportApi.Validators;

public class CreateReportRequestValidator : AbstractValidator<CreateReportRequest>
{
    public CreateReportRequestValidator()
    {
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Lokasyon alanı boş olamaz")
            .MaximumLength(100).WithMessage("Lokasyon en fazla 100 karakter olabilir")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s\-]+$").WithMessage("Lokasyon sadece harf, boşluk ve tire içerebilir");
    }
}

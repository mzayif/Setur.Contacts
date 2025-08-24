using FluentValidation;
using Setur.Contacts.Domain.Requests;

namespace Setur.Contacts.ContactApi.Validators;

public class CreateContactRequestValidator : AbstractValidator<CreateContactRequest>
{
    public CreateContactRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad alanı boş olamaz")
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Ad sadece harf içerebilir");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad alanı boş olamaz")
            .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Soyad sadece harf içerebilir");

        RuleFor(x => x.Company)
            .MaximumLength(100).WithMessage("Firma adı en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Company));
    }
}
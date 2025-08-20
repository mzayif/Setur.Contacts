using FluentValidation;
using Setur.Contacts.ContactApi.DTOs.Requests;

namespace Setur.Contacts.ContactApi.Validators
{
    public class CreateCommunicationInfoRequestValidator : AbstractValidator<CreateCommunicationInfoRequest>
    {
        public CreateCommunicationInfoRequestValidator()
        {
            RuleFor(x => x.ContactId)
                .NotEmpty().WithMessage("Kişi ID'si boş olamaz");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Geçersiz iletişim tipi");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("İletişim bilgisi değeri boş olamaz")
                .MaximumLength(100).WithMessage("İletişim bilgisi değeri en fazla 100 karakter olabilir")
                .When(x => x.Type == Domain.Enums.CommunicationType.Phone)
                .Matches(@"^[+]?[0-9\s\-\(\)]+$").WithMessage("Geçersiz telefon numarası formatı")
                .When(x => x.Type == Domain.Enums.CommunicationType.Email)
                .EmailAddress().WithMessage("Geçersiz e-posta adresi formatı");
        }
    }
}

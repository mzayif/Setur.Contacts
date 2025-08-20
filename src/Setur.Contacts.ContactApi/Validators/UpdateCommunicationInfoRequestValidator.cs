using FluentValidation;
using Setur.Contacts.ContactApi.DTOs.Requests;

namespace Setur.Contacts.ContactApi.Validators
{
    public class UpdateCommunicationInfoRequestValidator : AbstractValidator<UpdateCommunicationInfoRequest>
    {
        public UpdateCommunicationInfoRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID alanı boş olamaz");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Geçersiz iletişim tipi");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("İletişim bilgisi değeri boş olamaz")
                .MaximumLength(100).WithMessage("İletişim bilgisi değeri en fazla 100 karakter olabilir");

            // Telefon numarası validasyonu
            RuleFor(x => x.Value)
                .Matches(@"^[+]?[0-9\s\-\(\)]+$").WithMessage("Geçersiz telefon numarası formatı")
                .When(x => x.Type == Domain.Enums.CommunicationType.Phone);

            // Email validasyonu
            RuleFor(x => x.Value)
                .EmailAddress().WithMessage("Geçersiz e-posta adresi formatı")
                .When(x => x.Type == Domain.Enums.CommunicationType.Email);
        }
    }
}

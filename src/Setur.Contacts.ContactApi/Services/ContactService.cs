using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.Base.Results;
using Setur.Contacts.ContactApi.DTOs.Requests;
using Setur.Contacts.ContactApi.DTOs.Responses;
using Setur.Contacts.ContactApi.Repositories;
using Setur.Contacts.Domain.Entities;
using Mapster;

namespace Setur.Contacts.ContactApi.Services
{
    public class ContactService : IContactService
    {
        private readonly ContactRepository _contactRepository;

        public ContactService(ContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<SuccessDataResult<IEnumerable<ContactResponse>>> GetAllContactsAsync()
        {
            var contacts = await _contactRepository.GetWhere(includeProperties: "CommunicationInfos", isTracking: false)
                .ProjectToType<ContactResponse>()
                .ToListAsync();

            return new SuccessDataResult<IEnumerable<ContactResponse>>(contacts, contacts.Count);
        }

        public async Task<SuccessDataResult<ContactDetailResponse?>> GetContactByIdAsync(Guid id)
        {
            var contact = await _contactRepository.GetWhere(x => x.Id == id, includeProperties: "CommunicationInfos", isTracking: false)
                .FirstOrDefaultAsync();

            if (contact == null)
                throw new NotFoundException("Kişi Bulunamadı");

            var contactDetail = contact.Adapt<ContactDetailResponse>();

            return new SuccessDataResult<ContactDetailResponse?>(contactDetail);
        }

        public async Task<SuccessResponse> CreateContactAsync(CreateContactRequest request)
        {
            var contact = request.Adapt<Contact>();

            await _contactRepository.AddAsync(contact);
            await _contactRepository.SaveAsync();
            
            return new SuccessResponse(("1","Kişi başarıyla oluşturuldu"), contact.Id.ToString());
        }

        public async Task<SuccessResponse> UpdateContactAsync(Guid id, UpdateContactRequest request)
        {
            var contact = await _contactRepository.GetByIdAsync(id, throwException: false);
            if (contact == null)
                throw new NotFoundException("Kişi bulunamadı");

            request.Adapt(contact);

            _contactRepository.Update(contact);
            await _contactRepository.SaveAsync();

            return new SuccessResponse("Kişi başarıyla güncellendi");
        }

        public async Task<SuccessResponse> DeleteContactAsync(Guid id)
        {
            var contact = await _contactRepository.GetByIdAsync(id, throwException: false);
            if (contact == null)
                throw new NotFoundException("Kişi bulunamadı");

            _contactRepository.Remove(contact);
            await _contactRepository.SaveAsync();

            return new SuccessResponse("Kişi başarıyla silindi");
        }
    }
}
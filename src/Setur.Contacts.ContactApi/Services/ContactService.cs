using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.Base.Results;
using Setur.Contacts.ContactApi.Repositories;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.Domain.Enums;
using Mapster;
using ContactDetailResponse = Setur.Contacts.Domain.Responses.ContactDetailResponse;
using ContactResponse = Setur.Contacts.Domain.Responses.ContactResponse;
using CreateContactRequest = Setur.Contacts.Domain.Requests.CreateContactRequest;
using UpdateContactRequest = Setur.Contacts.Domain.Requests.UpdateContactRequest;
using PagedRequest = Setur.Contacts.Domain.Requests.PagedRequest;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.ContactApi.Services;

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

        return new SuccessResponse(("1", "Kişi başarıyla oluşturuldu"), contact.Id.ToString());
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

    public async Task<SuccessDataResult<ReportDataResponse>> GetReportDataAsync(ReportType reportType, List<string> filters)
    {
        var result = new ReportDataResponse
        {
            ReportType = reportType,
            Filters = filters
        };

        // IQueryable'ı başlat
        var contactsQuery = _contactRepository.GetWhere(includeProperties: "CommunicationInfos", isTracking: false);

        // Rapor türüne göre filtreleme - veritabanı seviyesinde (büyük/küçük harf duyarsız)
        contactsQuery = reportType switch
        {
            ReportType.LocationBased => filters.Any()
                ? contactsQuery
                    .Where(c => c.CommunicationInfos != null && c.CommunicationInfos.Any(ci => 
                    ci.Type == CommunicationType.Location && 
                    filters.Any(f => ci.Value.ToLower() == f.ToLower())))
                : contactsQuery,
            ReportType.CompanyBased => filters.Any()
                ? contactsQuery.Where(c => filters.Any(f => c.Company.ToLower() == f.ToLower()))
                : contactsQuery,
            ReportType.General => contactsQuery,
            _ => contactsQuery
        };

        // En son veri verilen koşullara göre DB den çağrılır
        var filteredContacts = await contactsQuery.ToListAsync();

        // Toplam sayıları hesapla
        result.TotalPersonCount = filteredContacts.Count;
        result.TotalPhoneCount = filteredContacts.SelectMany(c => c.CommunicationInfos).Count(ci => ci.Type == CommunicationType.Phone);
        result.TotalEmailCount = filteredContacts.SelectMany(c => c.CommunicationInfos).Count(ci => ci.Type == CommunicationType.Email);

        // Lokasyon bazlı detayları hesapla
        var locationGroups = filteredContacts
            .SelectMany(c => c.CommunicationInfos)
            .Where(c => c.Type == CommunicationType.Location)
            .GroupBy(ci => ci.Value)
            .Select(g => new ReportDetailData
            {
                Location = g.Key,
                PersonCount = g.Select(ci => ci.ContactId).Distinct().Count(),
                PhoneCount = 0,
                EmailCount = 0
            })
            .OrderByDescending(d => d.PersonCount)
            .ToList();

        foreach (var row in locationGroups)
        {
            row.EmailCount = filteredContacts
                .Where(x => x.CommunicationInfos.Any(y =>
                    y.Type == CommunicationType.Location && y.Value == row.Location)).Select(x =>
                    x.CommunicationInfos.Where(z => z.Type == CommunicationType.Email)).FirstOrDefault().Count();
            row.PhoneCount= filteredContacts
                .Where(x => x.CommunicationInfos.Any(y =>
                    y.Type == CommunicationType.Location && y.Value == row.Location)).Select(x =>
                    x.CommunicationInfos.Where(z => z.Type == CommunicationType.Phone)).FirstOrDefault().Count();
        }

        result.Details = locationGroups;
        result.TotalLocationCount = locationGroups.Count;

        // En çok kişi bulunan şirketler
        result.TopCompanies = filteredContacts
            .GroupBy(c => c.Company)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => g.Key)
            .ToList();

        // En çok kişi bulunan lokasyonlar
        result.TopLocations = locationGroups
            .OrderByDescending(d => d.PersonCount)
            .Take(3)
            .Select(d => d.Location)
            .ToList();

        return new SuccessDataResult<ReportDataResponse>(result,result.Details.Count);
    }

    public async Task<PagedResult<ContactResponse>> GetContactsPagedAsync(PagedRequest request)
    {
        // Toplam kayıt sayısını al
        var totalCount = await _contactRepository.GetAll().CountAsync();

        // Sayfalanmış verileri al
        var contacts = await _contactRepository.GetWhere(includeProperties: "CommunicationInfos", isTracking: false)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync();

        var contactResponses = contacts.Select(contact => new ContactResponse
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Company = contact.Company,
            CommunicationInfos = contact.CommunicationInfos?.Select(ci => new CommunicationInfoResponse
            {
                Id = ci.Id,
                Type = ci.Type,
                Value = ci.Value
            }).ToList() ?? new List<CommunicationInfoResponse>()
        }).ToList();

        return new PagedResult<ContactResponse>(contactResponses, totalCount, request.PageNumber, request.PageSize);
    }
}
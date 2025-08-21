using Setur.Contacts.Base.Results;
using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.ContactApi.DTOs.Requests;
using Setur.Contacts.ContactApi.DTOs.Responses;
using Setur.Contacts.Domain.Enums;
using Setur.Contacts.Domain.Models;

namespace Setur.Contacts.ContactApi.Services;

public interface IContactService
{
    /// <summary>
    /// Tüm kişileri iletişim bilgileriyle birlikte getirir
    /// </summary>
    /// <returns>Kişi listesi ve iletişim bilgileri</returns>
    Task<SuccessDataResult<IEnumerable<ContactResponse>>> GetAllContactsAsync();

    /// <summary>
    /// Belirtilen ID'ye sahip kişiyi iletişim bilgileriyle birlikte getirir. Kişiyi bulamazsa NotFoundException fırlatır.
    /// </summary>
    /// <param name="id">Kişi ID'si</param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns>Kişi detayları ve iletişim bilgileri</returns>
    Task<SuccessDataResult<ContactDetailResponse?>> GetContactByIdAsync(Guid id);

    /// <summary>
    /// Yeni bir kişi oluşturur
    /// </summary>
    /// <param name="request">Kişi oluşturma bilgileri</param>
    /// <returns>Oluşturulan kişi bilgileri</returns>
    Task<SuccessResponse> CreateContactAsync(CreateContactRequest request);

    /// <summary>
    /// Belirtilen ID'ye sahip kişiyi günceller. Kişiyi bulamazsa NotFoundException fırlatır.
    /// </summary>
    /// <param name="id">Kişi ID'si</param>
    /// <exception cref="NotFoundException"></exception>
    /// <param name="request">Güncellenecek kişi bilgileri</param>
    /// <returns>Güncelleme işlem sonucu</returns>
    Task<SuccessResponse> UpdateContactAsync(Guid id, UpdateContactRequest request);

    /// <summary>
    /// Belirtilen ID'ye sahip kişiyi siler. Kişiyi bulamazsa NotFoundException fırlatır.
    /// </summary>
    /// <param name="id">Kişi ID'si</param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns>Silme işlem sonucu</returns>
    Task<SuccessResponse> DeleteContactAsync(Guid id);

    /// <summary>
    /// Rapor verilerini getirir
    /// </summary>
    /// <param name="reportType">Rapor türü</param>
    /// <param name="filters">Filtre değerleri (lokasyonlar, şirketler vs.)</param>
    /// <returns>Rapor verileri</returns>
    Task<SuccessDataResult<ReportDataResponse>> GetReportDataAsync(ReportType reportType, List<string> filters);
}
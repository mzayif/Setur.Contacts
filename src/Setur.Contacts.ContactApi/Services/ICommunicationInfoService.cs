using Setur.Contacts.Base.Results;
using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.Domain.Requests;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.ContactApi.Services;

public interface ICommunicationInfoService
{
    /// <summary>
    /// Tüm iletişim bilgilerini getirir
    /// </summary>
    /// <returns>İletişim bilgileri listesi</returns>
    Task<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>> GetAllCommunicationInfosAsync();

    /// <summary>
    /// Belirtilen ID'ye sahip iletişim bilgisini getirir. İletişim bilgisini bulamazsa NotFoundException fırlatır.
    /// </summary>
    /// <param name="id">İletişim bilgisi ID'si</param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns>İletişim bilgisi detayları</returns>
    Task<SuccessDataResult<CommunicationInfoResponse?>> GetCommunicationInfoByIdAsync(Guid id);

    /// <summary>
    /// Belirtilen kişiye ait tüm iletişim bilgilerini getirir
    /// </summary>
    /// <param name="contactId">Kişi ID'si</param>
    /// <returns>Kişiye ait iletişim bilgileri listesi</returns>
    Task<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>> GetCommunicationInfosByContactIdAsync(Guid contactId);

    /// <summary>
    /// Yeni bir iletişim bilgisi oluşturur. Kişi bulunamazsa NotFoundException fırlatır.
    /// </summary>
    /// <param name="request">İletişim bilgisi oluşturma bilgileri</param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns>Oluşturma işlem sonucu</returns>
    Task<SuccessResponse> CreateCommunicationInfoAsync(CreateCommunicationInfoRequest request);

    /// <summary>
    /// Belirtilen ID'ye sahip iletişim bilgisini günceller. İletişim bilgisini bulamazsa NotFoundException fırlatır.
    /// </summary>
    /// <param name="id">İletişim bilgisi ID'si</param>
    /// <param name="request">Güncellenecek iletişim bilgileri</param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns>Güncelleme işlem sonucu</returns>
    Task<SuccessResponse> UpdateCommunicationInfoAsync(Guid id, UpdateCommunicationInfoRequest request);

    /// <summary>
    /// Belirtilen ID'ye sahip iletişim bilgisini siler. İletişim bilgisini bulamazsa NotFoundException fırlatır.
    /// </summary>
    /// <param name="id">İletişim bilgisi ID'si</param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns>Silme işlem sonucu</returns>
    Task<SuccessResponse> DeleteCommunicationInfoAsync(Guid id);
}
using Setur.Contacts.Base.Results;
using Setur.Contacts.Domain.Requests;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.BlazorApp.Services.Abstracts;

public interface ICommunicationInfoService
{
    /// <summary>
    /// Tüm iletişim bilgilerini getirir
    /// </summary>
    Task<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>> GetAllCommunicationInfosAsync();

    /// <summary>
    /// Belirtilen ID'ye sahip iletişim bilgisini getirir
    /// </summary>
    Task<SuccessDataResult<CommunicationInfoResponse?>> GetCommunicationInfoByIdAsync(Guid id);

    /// <summary>
    /// Belirtilen kişiye ait tüm iletişim bilgilerini getirir
    /// </summary>
    Task<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>> GetCommunicationInfosByContactIdAsync(Guid contactId);

    /// <summary>
    /// Yeni bir iletişim bilgisi oluşturur
    /// </summary>
    Task<SuccessResponse> CreateCommunicationInfoAsync(CreateCommunicationInfoRequest request);

    /// <summary>
    /// Belirtilen ID'ye sahip iletişim bilgisini günceller
    /// </summary>
    Task<SuccessResponse> UpdateCommunicationInfoAsync(Guid id, UpdateCommunicationInfoRequest request);

    /// <summary>
    /// Belirtilen ID'ye sahip iletişim bilgisini siler
    /// </summary>
    Task<SuccessResponse> DeleteCommunicationInfoAsync(Guid id);
}

using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.Base.Results;
using Setur.Contacts.ContactApi.Repositories;
using Setur.Contacts.Domain.Entities;
using Mapster;
using Setur.Contacts.Domain.Requests;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.ContactApi.Services;

public class CommunicationInfoService : ICommunicationInfoService
{
    private readonly CommunicationInfoRepository _communicationInfoRepository;
    private readonly ContactRepository _contactRepository;

    public CommunicationInfoService(CommunicationInfoRepository communicationInfoRepository, ContactRepository contactRepository)
    {
        _communicationInfoRepository = communicationInfoRepository;
        _contactRepository = contactRepository;
    }

    public async Task<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>> GetAllCommunicationInfosAsync()
    {
        var communicationInfos = await _communicationInfoRepository.GetAll(isTracking: false)
            .ProjectToType<CommunicationInfoResponse>()
            .ToListAsync();

        return new SuccessDataResult<IEnumerable<CommunicationInfoResponse>>(communicationInfos, communicationInfos.Count);
    }

    public async Task<SuccessDataResult<CommunicationInfoResponse?>> GetCommunicationInfoByIdAsync(Guid id)
    {
        var communicationInfo = await _communicationInfoRepository.GetByIdAsync(id, throwException: false, isTracking: false);
            
        if (communicationInfo == null)
            throw new NotFoundException("İletişim bilgisi bulunamadı");

        var response = communicationInfo.Adapt<CommunicationInfoResponse>();

        return new SuccessDataResult<CommunicationInfoResponse?>(response);
    }

    public async Task<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>> GetCommunicationInfosByContactIdAsync(Guid contactId)
    {
        var communicationInfos = await _communicationInfoRepository.GetWhere(x => x.ContactId == contactId, isTracking: false)
            .ProjectToType<CommunicationInfoResponse>()
            .ToListAsync();

        return new SuccessDataResult<IEnumerable<CommunicationInfoResponse>>(communicationInfos, communicationInfos.Count);
    }

    public async Task<SuccessResponse> CreateCommunicationInfoAsync(CreateCommunicationInfoRequest request)
    {
        var contact = await _contactRepository.GetByIdAsync(request.ContactId, throwException: false);
        if (contact == null)
            throw new NotFoundException("Kişi bulunamadı");

        // Aynı kişi için aynı tip ve değerde iletişim bilgisi var mı kontrol et
        var existingCommunicationInfo = await _communicationInfoRepository.GetWhere(
            x => x.ContactId == request.ContactId && 
                 x.Type == request.Type && 
                 x.Value.ToLower() == request.Value.ToLower(), 
            isTracking: false)
            .FirstOrDefaultAsync();

        if (existingCommunicationInfo != null)
            throw new BusinessException("Bu iletişim bilgisi zaten mevcut");

        var communicationInfo = request.Adapt<CommunicationInfo>();

        await _communicationInfoRepository.AddAsync(communicationInfo);
        await _communicationInfoRepository.SaveAsync();
            
        return new SuccessResponse("İletişim bilgisi başarıyla oluşturuldu");
    }

    public async Task<SuccessResponse> UpdateCommunicationInfoAsync(Guid id, UpdateCommunicationInfoRequest request)
    {
        var communicationInfo = await _communicationInfoRepository.GetByIdAsync(id, throwException: false);
        if (communicationInfo == null)
            throw new NotFoundException("İletişim bilgisi bulunamadı");

        request.Adapt(communicationInfo);

        _communicationInfoRepository.Update(communicationInfo);
        await _communicationInfoRepository.SaveAsync();

        return new SuccessResponse("İletişim bilgisi başarıyla güncellendi");
    }

    public async Task<SuccessResponse> DeleteCommunicationInfoAsync(Guid id)
    {
        var communicationInfo = await _communicationInfoRepository.GetByIdAsync(id, throwException: false);
        if (communicationInfo == null)
            throw new NotFoundException("İletişim bilgisi bulunamadı");

        _communicationInfoRepository.Remove(communicationInfo);
        await _communicationInfoRepository.SaveAsync();

        return new SuccessResponse("İletişim bilgisi başarıyla silindi");
    }
}
using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.Base.Results;
using Setur.Contacts.ContactApi.DTOs.Requests;
using Setur.Contacts.ContactApi.DTOs.Responses;
using Setur.Contacts.ContactApi.Repositories;
using Setur.Contacts.Domain.Entities;

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
            .Select(ci => new CommunicationInfoResponse
            {
                Id = ci.Id,
                Type = ci.Type,
                Value = ci.Value
            })
            .ToListAsync();

        return new SuccessDataResult<IEnumerable<CommunicationInfoResponse>>(communicationInfos, communicationInfos.Count);
    }

    public async Task<SuccessDataResult<CommunicationInfoResponse?>> GetCommunicationInfoByIdAsync(Guid id)
    {
        var communicationInfo = await _communicationInfoRepository.GetByIdAsync(id, throwException: false, isTracking: false);
            
        if (communicationInfo == null)
            throw new NotFoundException("İletişim bilgisi bulunamadı");

        var response = new CommunicationInfoResponse
        {
            Id = communicationInfo.Id,
            Type = communicationInfo.Type,
            Value = communicationInfo.Value
        };

        return new SuccessDataResult<CommunicationInfoResponse?>(response);
    }

    public async Task<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>> GetCommunicationInfosByContactIdAsync(Guid contactId)
    {
        var communicationInfos = await _communicationInfoRepository.GetWhere(x => x.ContactId == contactId, isTracking: false)
            .Select(ci => new CommunicationInfoResponse
            {
                Id = ci.Id,
                Type = ci.Type,
                Value = ci.Value
            })
            .ToListAsync();

        return new SuccessDataResult<IEnumerable<CommunicationInfoResponse>>(communicationInfos, communicationInfos.Count);
    }

    public async Task<SuccessResponse> CreateCommunicationInfoAsync(CreateCommunicationInfoRequest request)
    {
        var contact = await _contactRepository.GetByIdAsync(request.ContactId, throwException: false);
        if (contact == null)
            throw new NotFoundException("Kişi bulunamadı");

        var communicationInfo = new CommunicationInfo
        {
            ContactId = request.ContactId,
            Type = request.Type,
            Value = request.Value
        };

        await _communicationInfoRepository.AddAsync(communicationInfo);
        await _communicationInfoRepository.SaveAsync();
            
        return new SuccessResponse("İletişim bilgisi başarıyla oluşturuldu");
    }

    public async Task<SuccessResponse> UpdateCommunicationInfoAsync(Guid id, UpdateCommunicationInfoRequest request)
    {
        var communicationInfo = await _communicationInfoRepository.GetByIdAsync(id, throwException: false);
        if (communicationInfo == null)
            throw new NotFoundException("İletişim bilgisi bulunamadı");

        communicationInfo.Type = request.Type;
        communicationInfo.Value = request.Value;

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
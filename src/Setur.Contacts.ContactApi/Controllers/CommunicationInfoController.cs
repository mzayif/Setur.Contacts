using Microsoft.AspNetCore.Mvc;
using Setur.Contacts.ContactApi.Services;
using Setur.Contacts.Domain.Requests;

namespace Setur.Contacts.ContactApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommunicationInfoController : ControllerBase
{
    private readonly ICommunicationInfoService _communicationInfoService;

    public CommunicationInfoController(ICommunicationInfoService communicationInfoService)
    {
        _communicationInfoService = communicationInfoService;
    }

    // GET: api/communication-info
    [HttpGet]
    public async Task<IActionResult> GetCommunicationInfos()
    {
        var result = await _communicationInfoService.GetAllCommunicationInfosAsync();
        return Ok(result);
    }

    // GET: api/communication-info/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommunicationInfo(Guid id)
    {
        var result = await _communicationInfoService.GetCommunicationInfoByIdAsync(id);
        return Ok(result);
    }

    // GET: api/communication-info/contacts/{contactId}
    [HttpGet("contacts/{contactId}")]
    public async Task<IActionResult> GetContactCommunicationInfos(Guid contactId)
    {
        var result = await _communicationInfoService.GetCommunicationInfosByContactIdAsync(contactId);
        return Ok(result);
    }

    // POST: api/communication-info
    [HttpPost]
    public async Task<IActionResult> CreateCommunicationInfo(CreateCommunicationInfoRequest request)
    {
        var result = await _communicationInfoService.CreateCommunicationInfoAsync(request);
        return Ok(result);
    }

    // PUT: api/communication-info/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCommunicationInfo(Guid id, UpdateCommunicationInfoRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        var result = await _communicationInfoService.UpdateCommunicationInfoAsync(id, request);
        return Ok(result);
    }

    // DELETE: api/communication-info/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCommunicationInfo(Guid id)
    {
        var result = await _communicationInfoService.DeleteCommunicationInfoAsync(id);
        return Ok(result);
    }
}
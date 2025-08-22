using Microsoft.AspNetCore.Mvc;
using Setur.Contacts.ContactApi.Services;
using Setur.Contacts.Domain.Requests;

namespace Setur.Contacts.ContactApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    // GET: api/contacts
    [HttpGet]
    public async Task<IActionResult> GetContacts()
    {
        var result = await _contactService.GetAllContactsAsync();
        return Ok(result);
    }

    // GET: api/contacts/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetContact(Guid id)
    {
        var result = await _contactService.GetContactByIdAsync(id);
        return Ok(result);
    }

    // POST: api/contacts
    [HttpPost]
    public async Task<IActionResult> CreateContact(CreateContactRequest request)
    {
        var result = await _contactService.CreateContactAsync(request);
        return Ok(result);
    }

    // PUT: api/contacts/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContact(Guid id, UpdateContactRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        var result = await _contactService.UpdateContactAsync(id, request);
        return Ok(result);
    }

    // DELETE: api/contacts/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(Guid id)
    {
        var result = await _contactService.DeleteContactAsync(id);
        return Ok(result);
    }
}
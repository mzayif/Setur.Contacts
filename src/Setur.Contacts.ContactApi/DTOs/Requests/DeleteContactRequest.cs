using System;
using System.ComponentModel.DataAnnotations;

namespace Setur.Contacts.ContactApi.DTOs.Requests;

public class DeleteContactRequest
{
    [Required]
    public Guid Id { get; set; }
}
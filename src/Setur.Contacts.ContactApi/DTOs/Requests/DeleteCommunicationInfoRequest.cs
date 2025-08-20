using System;
using System.ComponentModel.DataAnnotations;

namespace Setur.Contacts.ContactApi.DTOs.Requests;

public class DeleteCommunicationInfoRequest
{
    [Required]
    public Guid Id { get; set; }
}
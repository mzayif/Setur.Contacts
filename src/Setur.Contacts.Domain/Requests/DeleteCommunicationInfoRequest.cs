using System.ComponentModel.DataAnnotations;

namespace Setur.Contacts.Domain.Requests;

public class DeleteCommunicationInfoRequest
{
    [Required]
    public Guid Id { get; set; }
}
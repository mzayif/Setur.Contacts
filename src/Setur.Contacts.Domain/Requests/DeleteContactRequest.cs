using System.ComponentModel.DataAnnotations;

namespace Setur.Contacts.Domain.Requests;

public class DeleteContactRequest
{
    [Required]
    public Guid Id { get; set; }
}
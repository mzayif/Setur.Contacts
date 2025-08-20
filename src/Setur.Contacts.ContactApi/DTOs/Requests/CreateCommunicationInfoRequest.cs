using Setur.Contacts.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Setur.Contacts.ContactApi.DTOs.Requests
{
    public class CreateCommunicationInfoRequest
    {
        [Required]
        public Guid ContactId { get; set; }

        [Required]
        public CommunicationType Type { get; set; }

        [Required]
        [StringLength(100)]
        public string Value { get; set; }
    }
}

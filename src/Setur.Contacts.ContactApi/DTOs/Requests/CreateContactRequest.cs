using System.ComponentModel.DataAnnotations;

namespace Setur.Contacts.ContactApi.DTOs.Requests
{
    public class CreateContactRequest
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Company { get; set; }
    }
}

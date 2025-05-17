using System.ComponentModel.DataAnnotations;

namespace APIs.DTOs
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

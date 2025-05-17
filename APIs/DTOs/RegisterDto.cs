using System.ComponentModel.DataAnnotations;

namespace APIs.DTOs
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string CheckPassword { get; set; }
    }
}

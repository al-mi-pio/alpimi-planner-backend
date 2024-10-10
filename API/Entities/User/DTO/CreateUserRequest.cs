using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.User.DTO
{
    public class CreateUserRequest
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string CustomURL { get; set; }
    }
}

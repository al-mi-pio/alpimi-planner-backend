using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.User.DTO
{
    public class CreateUserDTO
    {
        [Required]
        public required string Login { get; set; }

        [Required]
        public required string CustomURL { get; set; }
    }
}

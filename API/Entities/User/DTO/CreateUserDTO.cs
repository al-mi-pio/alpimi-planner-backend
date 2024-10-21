using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.User.DTO
{
    public class CreateUserDTO
    {
        [Required]
        public required string Login { get; set; }

        [Required]
        public required string CustomURL { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}

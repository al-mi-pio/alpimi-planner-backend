using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.User.DTO
{
    public class CreateUserDTO
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string CustomURL { get; set; }
    }
}

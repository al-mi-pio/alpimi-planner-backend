using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.User.DTO
{
    public class UpdateUserDTO
    {
        public required string? Login { get; set; }

        public required string? CustomURL { get; set; }
    }
}

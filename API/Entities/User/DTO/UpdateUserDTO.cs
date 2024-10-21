using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.User.DTO
{
    public class UpdateUserDTO
    {
        public required string? Login { get; set; }

        public required string? CustomURL { get; set; }
    }
}

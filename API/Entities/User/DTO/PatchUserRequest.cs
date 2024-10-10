using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.User.DTO
{
    public class PatchUserRequest
    {
        public string? Login { get; set; }

        public string? CustomURL { get; set; }
    }
}

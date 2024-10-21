using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.EAuth.DTO
{
    public record LoginDTO
    {
        [Required]
        public required string Login { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}

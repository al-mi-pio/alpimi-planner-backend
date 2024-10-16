using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Auth.DTO
{
    public record CreateBreedRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

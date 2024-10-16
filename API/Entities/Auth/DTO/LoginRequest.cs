using System.ComponentModel.DataAnnotations;

namespace alpimi_planner_backend.API.Entities.Auth.DTO
{
    public record CreateBreedRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

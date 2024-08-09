using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Breed.Requests
{
    public record CreateBreedRequest
    {
        [Required]
        public string Name { get; set; }
        public string? CountryOrigin { get; set; }
    }
}

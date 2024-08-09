using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Dog.Requests
{
    public record CreateDogRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public int BreedId { get; set; }
    }
}

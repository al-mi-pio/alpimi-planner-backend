using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ECollisionType.DTO
{
    public class UpdateCollisionTypeDTO
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? Weight { get; set; }

        public string? Filter { get; set; }

        public string? Category { get; set; }
    }
}

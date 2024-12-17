using AlpimiAPI.Entities.EGroup;

namespace AlpimiAPI.Entities.ESubgroup
{
    public class Subgroup
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required int StudentCount { get; set; }

        public required Guid GroupId { get; set; }

        public required Group Group { get; set; }
    }
}

using AlpimiAPI.Entities.EStudent;

namespace AlpimiAPI.Relations
{
    public class StudentSubgroup
    {
        public Guid Id { get; set; }
        public required Guid StudentId { get; set; }
        public required Student Student { get; set; }
        public required Guid SubgroupId { get; set; }
    }
}

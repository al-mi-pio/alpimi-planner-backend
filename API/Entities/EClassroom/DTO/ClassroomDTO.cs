namespace AlpimiAPI.Entities.EClassroom.DTO
{
    public class ClassroomDTO
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required int Capacity { get; set; }
    }
}

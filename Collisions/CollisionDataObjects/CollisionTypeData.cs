using AlpimiAPI.Entities.ESchedule;

namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionTypeData
    {
        public Guid id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public Double weight { get; set; }

        public Filter filter { get; set; }

        public string category { get; set; }

        public CollisionTypeData(
            Guid id,
            string name,
            string description,
            Double weight,
            Filter filter,
            string category
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.weight = weight;
            this.filter = filter;
            this.category = category;
        }
    }
}

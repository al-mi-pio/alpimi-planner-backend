using AlpimiAPI.Entities.EAvailability;
using AlpimiAPI.Entities.ECollisionType;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.EStudent;
using AlpimiAPI.Entities.ESubgroup;

namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionScanStaticData
    {
        public IEnumerable<LessonPeriod>? LessonPeriods { get; set; }
        public IEnumerable<Availability>? Availabilities { get; set; }
        public IEnumerable<Group>? Groups { get; set; }
        public IEnumerable<Subgroup>? Subgroups { get; set; }
        public IEnumerable<CollisionType>? CollisionTypes { get; set; }
    }
}

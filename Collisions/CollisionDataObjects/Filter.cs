namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class Filter
    {
        public List<RuleData> objectRules { get; set; }
        public List<RuleData> targetRules { get; set; }
        public List<RuleData> methodRules { get; set; }

        public Filter()
        {
            objectRules = new List<RuleData>();
            targetRules = new List<RuleData>();
            methodRules = new List<RuleData>();
        }
    }
}

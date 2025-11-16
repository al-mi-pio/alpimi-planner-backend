namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class RuleData
    {
        public string name { get; set; }
        public string data { get; set; }

        public RuleData(string name, string data)
        {
            this.name = name;
            this.data = data;
        }
    }
}

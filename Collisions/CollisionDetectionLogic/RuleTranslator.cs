using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.RuleLibrary;

namespace alpimi_planner_backend.Collisions.CollisionDetectionLogic
{
    public class RuleTranslator
    {
        public static List<Rule> GetObjectRules(List<RuleData> ruleDataList)
        {
            List<Rule> ruleList = new List<Rule>();
            foreach (RuleData rule in ruleDataList)
            {
                switch (rule.name)
                {
                    default:
                        ruleList.Add(new Rule(rule.data));
                        break;
                }
            }
            return ruleList;
        }

        public static List<Rule> GetTargetRules(List<RuleData> ruleDataList)
        {
            List<Rule> ruleList = new List<Rule>();
            foreach (RuleData rule in ruleDataList)
            {
                switch (rule.name)
                {
                    default:
                        ruleList.Add(new Rule(rule.data));
                        break;
                }
            }
            return ruleList;
        }

        public static List<Rule> GetMethodRules(List<RuleData> ruleDataList)
        {
            List<Rule> ruleList = new List<Rule>();
            foreach (RuleData rule in ruleDataList)
            {
                switch (rule.name)
                {
                    case "test":
                        ruleList.Add(new TestRule(rule.data));
                        break;
                    case "coincide":
                        ruleList.Add(new CoincideRule(rule.data));
                        break;
                    case "sameteacher":
                        ruleList.Add(new SameTeacherRule(rule.data));
                        break;
                    default:
                        ruleList.Add(new Rule(rule.data));
                        break;
                }
            }
            return ruleList;
        }
    }
}

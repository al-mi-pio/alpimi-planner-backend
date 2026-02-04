using System.Text.Json;
using AlpimiAPI.Entities.EScheduleSettings;
using alpimi_planner_backend.Collisions.CollisionDataObjects;
using alpimi_planner_backend.Collisions.CollisionUtils;

namespace alpimi_planner_backend.Collisions.RuleLibrary
{
    public class TestRule : Rule
    {
        public record TestRuleParams(int liczba, int liczba2, string text);

        public TestRuleParams? Params { get; set; }

        public TestRule(string data)
            : base(data)
        {
            this.Params = JsonSerializer.Deserialize<TestRuleParams>(data);
        }

        public override bool CheckBlock(
            CollisionLessonBlock objectBlock,
            DateOnly scanDate,
            CollisionScanBlocks collisionScanBlocks,
            ScheduleSettings scheduleSettings,
            CollisionScanStaticData collisionScanStaticData,
            LogMaker logMaker
        )
        {
            if (Params != null)
            {
                logMaker.writeLog(
                    "To jest liczba: "
                        + Params.liczba
                        + " To jest kolejna liczba: "
                        + Params.liczba2
                        + " To jest suma: "
                        + (Params.liczba + Params.liczba2)
                        + " To jest tekst: "
                        + Params.text
                );
            }
            if (objectBlock.Lesson != null)
            {
                if (objectBlock.Lesson.Teacher.Name == "Jan Kowalski")
                {
                    return true;
                }
            }

            return false;
        }
    }
}

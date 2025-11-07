namespace alpimi_planner_backend.Collisions.CollisionUtils
{
    public class DateOnlyUtils
    {
        public static DateOnly getWeekStart(DateOnly currentDay)
        {
            int timeDifference = (7 + (currentDay.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateOnly weekStart = currentDay.AddDays(-timeDifference);
            return weekStart;
        }

        public static DateOnly getWeekEnd(DateOnly currentDay)
        {
            int timeDifference = (7 + (DayOfWeek.Sunday - currentDay.DayOfWeek)) % 7;
            DateOnly weekStart = currentDay.AddDays(timeDifference);
            return weekStart;
        }
    }
}

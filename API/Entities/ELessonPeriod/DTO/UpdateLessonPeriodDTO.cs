﻿namespace AlpimiAPI.Entities.ELessonPeriod.DTO
{
    public class UpdateLessonPeriodDTO
    {
        public TimeOnly? Start { get; set; }

        public TimeOnly? Finish { get; set; }
    }
}
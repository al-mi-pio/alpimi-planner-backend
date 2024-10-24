﻿using AlpimiAPI.Entities.EUser;

namespace AlpimiAPI.Entities.ESchedule
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required int SchoolHour { get; set; }
        public required Guid UserID { get; set; }
        public required User User { get; set; }
    }
}
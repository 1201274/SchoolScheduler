using System.Collections.Generic;
using SchoolScheduler.Models;
using Day = SchoolScheduler.Models.Day;

namespace SchoolScheduler.Core.Initializers
{
    public class TimeSlotInitializer
    {
        public List<TimeSlot> Initialize()
        {
            var timeSlots = new List<TimeSlot>();
            foreach (var day in new[] { Day.MON, Day.TUE, Day.WED, Day.THU, Day.FRI })
                for (int hour = 8; hour <= 17; hour++)
                { 
                    timeSlots.Add(new TimeSlot(day, new TimeOnly(hour, 0), 30));
                    timeSlots.Add(new TimeSlot(day, new TimeOnly(hour, 30), 30));
                }

            for (int hour = 8; hour <= 12; hour++)
            {
                timeSlots.Add(new TimeSlot(Day.SAT, new TimeOnly(hour, 0), 30));
                timeSlots.Add(new TimeSlot(Day.SAT, new TimeOnly(hour, 30), 30));
            }
            return timeSlots;
        }
    }
}

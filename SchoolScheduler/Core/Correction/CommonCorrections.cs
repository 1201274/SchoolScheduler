using SchoolScheduler.Models;

namespace SchoolScheduler.Core.Correction
{
    public class CommonCorrections()
    {
        public static TimeSlot? FindNextAvailableSlot(Assignment assignment, List<Assignment> allAssignments, List<TimeSlot> timeSlots, bool sameDayOnly = false)
        {
            var filteredSlots = sameDayOnly
                ? timeSlots.Where(ts => ts.Day == assignment.TimeSlot.Day)
                : timeSlots;

            foreach (var slot in filteredSlots)
            {
                bool teacherFree = !allAssignments.Any(a =>
                    a.Teacher == assignment.Teacher &&
                    a.TimeSlot == slot);

                bool classFree = !allAssignments.Any(a =>
                    a.Class == assignment.Class &&
                    a.TimeSlot == slot);

                if (teacherFree && classFree)
                    return slot;
            }

            return null;
        }
    }
}
using SchoolScheduler.Models;
using SchoolScheduler.Utils;

namespace SchoolScheduler.Core.Correction
{
    public static class TeacherCorrections
    {
        public static void FixConflicts(List<Assignment> assignments, List<TimeSlot> allTimeSlots)
        {
            var conflicts = assignments
                .GroupBy(a => new { a.TimeSlot, a.Teacher })
                .Where(g => g.Count() > 1);

            foreach (var group in conflicts)
            {
                var excess = group.Skip(1).ToList();
                bool slotNotFound = false;

                foreach (var assignment in excess)
                {
                    // 1. Try assigning another available teacher for the same time slot
                    var availableTeacher = assignment.ClassType.TeacherNames
                        .FirstOrDefault(t =>
                            t != assignment.Teacher &&
                            !assignments.Any(a =>
                                a.Teacher == t &&
                                a.TimeSlot == assignment.TimeSlot));

                    if (availableTeacher != null)
                    {
                        assignment.Teacher = availableTeacher;
                        continue;
                    }

                    // 2. Try moving the assignment to another time slot where teacher and class are both free
                    var newSlot = CommonCorrections.FindNextAvailableSlot(assignment, assignments, allTimeSlots);
                    if (newSlot != null)
                    {
                        assignment.TimeSlot = newSlot;
                    }
                    else
                        slotNotFound = true;
                }

                // Try moving the first assignment to another time slot where teacher and class are both free if at least one change fails 
                if (slotNotFound)
                {
                    var newSlot = CommonCorrections.FindNextAvailableSlot(group.First(), assignments, allTimeSlots);
                    if (newSlot != null)
                    {
                        group.First().TimeSlot = newSlot;
                    }
                }
            }
        }

        public static void FixLunchBreaks(List<Assignment> assignments, List<TimeSlot> allTimeSlots)
        {
            var groups = assignments.GroupBy(a => new { a.Teacher, a.TimeSlot.Day })
                .Select(g => g.ToList()).ToList();
            var rng = new Random();

            foreach (var group in groups)
            {
                var hasMorning = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.MORNING);
                var hasAfternoon = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.AFTERNOON);

                if (!hasMorning || !hasAfternoon)
                    continue;

                ConstraintUtils.GetMaxGap(group, Constants.LunchStartTime, Constants.LunchEndTime, out TimeSpan maxGap);

                if (maxGap >= Constants.MinMealBreakDuration)
                    continue;

                var lunchAssignments = group
                    .Where(a => a.TimeSlot.StartTime >= Constants.LunchStartTime &&
                                a.TimeSlot.EndTime <= Constants.LunchEndTime)
                    .ToList();

                if (lunchAssignments.Count == 0)
                    continue;

                foreach (var assignmentToMove in lunchAssignments.OrderBy(_ => rng.Next()))
                {
                    ConstraintUtils.GetMaxGap(group.Except(new [] { assignmentToMove }).ToList(), Constants.LunchStartTime, Constants.LunchEndTime, out TimeSpan maxGapUpdated);

                    if (maxGapUpdated < Constants.MinMealBreakDuration)
                        continue;

                    var newSlot = CommonCorrections.FindNextAvailableSlot(assignmentToMove, assignments, allTimeSlots);
                    if (newSlot != null)
                    {
                        assignmentToMove.TimeSlot = newSlot;
                        break;
                    }
                }
            }
        }

        public static void FixDinnerBreaks(List<Assignment> assignments, List<TimeSlot> allTimeSlots)
        {
            var groups = assignments.GroupBy(a => new { a.Teacher, a.TimeSlot.Day })
                .Select(g => g.ToList()).ToList();
            var rng = new Random();

            foreach (var group in groups)
            {
                var hasLateClass = group.Any(a => a.TimeSlot.StartTime >= new TimeOnly(21, 0));

                if (!hasLateClass)
                    continue;

                    
                ConstraintUtils.GetMaxGap(group, Constants.LunchStartTime, Constants.LunchEndTime, out TimeSpan maxGap);

                if (maxGap >= Constants.MinMealBreakDuration)
                    continue;

                var lunchAssignments = group
                    .Where(a => a.TimeSlot.StartTime >= Constants.LunchStartTime &&
                                a.TimeSlot.EndTime <= Constants.LunchEndTime)
                    .ToList();

                if (lunchAssignments.Count == 0)
                    continue;

                foreach (var assignmentToMove in lunchAssignments.OrderBy(_ => rng.Next()))
                {
                    ConstraintUtils.GetMaxGap(group.Except(new [] { assignmentToMove }).ToList(), Constants.LunchStartTime, Constants.LunchEndTime, out TimeSpan maxGapUpdated);

                    if (maxGapUpdated < Constants.MinMealBreakDuration)
                        continue;

                    var newSlot = CommonCorrections.FindNextAvailableSlot(assignmentToMove, assignments, allTimeSlots);
                    if (newSlot != null)
                    {
                        assignmentToMove.TimeSlot = newSlot;
                        break;
                    }
                }
            }
        }

        public static void FixMaxHoursPerDay(List<Assignment> assignments, List<TimeSlot> allTimeSlots)
        {
            var groups = assignments.GroupBy(a => new { a.Teacher, a.TimeSlot.Day })
                .Select(g => g.ToList()).ToList();

            foreach (var group in groups)
            {
                if (group.Sum(a => a.TimeSlot.Time) / 60 <= 8)
                    continue;

                var orderedGroup = group.OrderBy(a => a.TimeSlot).ToList();

                int left = 0;
                int right = orderedGroup.Count - 1;

                while (orderedGroup.Sum(a => a.TimeSlot.Time) / 60 > 8 && left <= right)
                {
                    var candidates = new[] { orderedGroup[left], orderedGroup[right] };

                    foreach (var assignment in candidates)
                    {
                        // Skip if already under max
                        if (group.Sum(a => a.TimeSlot.Time) / 60 <= 8)
                            break;

                        // Try to find another teacher for the same slot
                        var replacementTeacher = assignment.ClassType.TeacherNames
                            .FirstOrDefault(t =>
                                t != assignment.Teacher &&
                                !assignments.Any(a =>
                                    a.Teacher == t &&
                                    a.TimeSlot == assignment.TimeSlot));

                        if (replacementTeacher != null)
                        {
                            assignment.Teacher = replacementTeacher;
                        }
                        else
                        {
                            // Try moving to a new time slot
                            var newSlot = CommonCorrections.FindNextAvailableSlot(assignment, assignments, allTimeSlots);
                            if (newSlot != null)
                            {
                                assignment.TimeSlot = newSlot;
                            }
                        }
                    }

                    left++;
                    right--;
                }
            }
        }

    }
}
using SchoolScheduler.Models;
using SchoolScheduler.Utils;

namespace SchoolScheduler.Core.Correction
{
    public static class StudentCorrections
    {
        public static void FixStudentLunchBreaks(List<Assignment> assignments, List<TimeSlot> allTimeSlots)
        {
            var rng = new Random();

            var groups = assignments
                .SelectMany(a => a.Class.ClassNames.Select(letter => new
                {
                    Key = (a.Subject.Course, Letter: letter, a.TimeSlot.Day),
                    Assignment = a
                }))
                .GroupBy(x => x.Key)
                .Select(g => g.Select(x => x.Assignment).ToList())
                .ToList();

            foreach (var group in groups)
            {
                bool hasMorning = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.MORNING);
                bool hasAfternoon = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.AFTERNOON);


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

        public static void FixStudentDinnerBreaks(List<Assignment> assignments, List<TimeSlot> allTimeSlots)
        {
            var rng = new Random();

            var groups = assignments
                .SelectMany(a => a.Class.ClassNames.Select(letter => new
                {
                    Key = (a.Subject.Course, Letter: letter, a.TimeSlot.Day),
                    Assignment = a
                }))
                .GroupBy(x => x.Key)
                .Select(g => g.Select(x => x.Assignment).ToList())
                .ToList();

            foreach (var group in groups)
            {
                var hasLateClass = group.Any(a => a.TimeSlot.StartTime >= new TimeOnly(21, 0));

                if (!hasLateClass)
                    continue;

                ConstraintUtils.GetMaxGap(group, Constants.DinnerStartTime, Constants.DinnerEndTime, out TimeSpan maxGap);

                if (maxGap >= Constants.MinMealBreakDuration)
                    continue;

                var lunchAssignments = group
                    .Where(a => a.TimeSlot.StartTime >= Constants.DinnerStartTime &&
                                a.TimeSlot.EndTime <= Constants.DinnerEndTime)
                    .ToList();

                if (lunchAssignments.Count == 0)
                    continue;

                foreach (var assignmentToMove in lunchAssignments.OrderBy(_ => rng.Next()))
                {
                    ConstraintUtils.GetMaxGap(group.Except(new [] { assignmentToMove }).ToList(), Constants.DinnerStartTime, Constants.DinnerEndTime, out TimeSpan maxGapUpdated);

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

        public static void FixStudentMaxHoursPerDay(List<Assignment> assignments, List<TimeSlot> allTimeSlots)
        {
            var groups = assignments
                .SelectMany(a => a.Class.ClassNames.Select(letter => new
                {
                    Key = (a.Subject.Course, Letter: letter, a.TimeSlot.Day),
                    Assignment = a
                }))
                .GroupBy(x => x.Key)
                .Select(g => g.Select(x => x.Assignment).ToList())
                .ToList();

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
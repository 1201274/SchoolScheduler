using SchoolScheduler.Models;
using SchoolScheduler.Utils;

namespace SchoolScheduler.Core.Constraints
{
    public static class SoftConstraints
    {

        public static List<Constraint> GetNamedConstraints()
        {
            return new List<Constraint>
            {
                new Constraint { Name = ConstraintName.StudentGaps, Func = StudentGaps },
                new Constraint { Name = ConstraintName.IncorrectOrderOfClasses, Func = IncorrectOrderOfClasses },
                new Constraint { Name = ConstraintName.OutOfDepartmentRoomUsage, Func = OutOfDepartmentRoomUsage },
                new Constraint { Name = ConstraintName.PreferredRoomTypeConstraint, Func = PreferredRoomTypeConstraint },
                new Constraint { Name = ConstraintName.StudentsMin3HoursDay, Func = StudentsMin3HoursDay },
                new Constraint { Name = ConstraintName.ContinuousClassGaps, Func = ContinuousClassGaps },
                new Constraint { Name = ConstraintName.ContinuousClassDifferentTeachers, Func = ContinuousClassDifferentTeachers },
                new Constraint { Name = ConstraintName.ContinuousClassDifferentRooms, Func = ContinuousClassDifferentRooms },
                new Constraint { Name = ConstraintName.MultipleAssignmentsInSameTimeSlotForClasses, Func = MultipleAssignmentsInSameTimeSlotForClasses },
            };
        }

        public static List<Delegate> GetConstraints()
        {
            return GetNamedConstraints().Select(c => c.Func).ToList();
        }

        // 1. Avoid schedule gaps for students
        public static readonly Func<List<Assignment>, int> StudentGaps = assignments =>
        {
            int gaps = 0;

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
                var allGaps = ConstraintUtils.GetGaps(group);
                
                ConstraintUtils.GetMaxGap(group, Constants.LunchStartTime, Constants.LunchEndTime, out var lunchGap);
                ConstraintUtils.GetMaxGap(group, Constants.DinnerStartTime, Constants.DinnerEndTime, out var dinnerGap);

                if (lunchGap >= Constants.MinMealBreakDuration) allGaps.Remove(lunchGap);
                if (dinnerGap >= Constants.MinMealBreakDuration) allGaps.Remove(dinnerGap);

                foreach (var gap in allGaps)
                {
                    gaps += (int) Math.Floor(gap / TimeSpan.FromMinutes(30));

                }
            }

            return gaps;
        };



        // 2. T before TP before PL
        public static readonly Func<List<Assignment>, int> IncorrectOrderOfClasses = assignments =>
        {
            int violations = 0;
            var groups = assignments
                .SelectMany(a => a.Class.ClassNames.Select(letter => new
                {
                    Key = (a.Subject, Letter: letter),
                    Assignment = a
                }))
                .GroupBy(x => x.Key)
                .Select(g => g.Select(x => x.Assignment).ToList())
                .ToList();

            foreach (var group in groups)
            {
                var ordered = group.OrderBy(a => a.TimeSlot).ToList();
                for (int i = 1; i < ordered.Count - 1; i++)
                {
                    var diff = ordered[i - 1].ClassType.Type - ordered[i].ClassType.Type;
                    if (diff > 0) violations += diff;
                }
            }
            return violations;
        };

        // // 3. Cisco group classes should be in the morning
        // public static Func<List<Assignment>, int> CiscoMorningOnly(string ciscoGroupName) => assignments =>
        //     assignments.Count(a => a.Subject.Group == ciscoGroupName && a.TimeSlot.Hour >= 13);

        // 4. Penalize use of rooms from other departments
        public static readonly Func<List<Assignment>, int> OutOfDepartmentRoomUsage = assignments =>
            assignments.Count(a => a.Room.Department != a.Subject.Department);

        // // 5. Teacher prefers fewer/more days
        // public static readonly Func<List<Assignment>, int> TeacherDayPreferences = assignments =>
        // {
        //     int penalty = 0;
        //     var teacherDays = assignments
        //         .GroupBy(a => a.Teacher.Name)
        //         .ToDictionary(
        //             g => g.Key,
        //             g => g.Select(a => a.TimeSlot.Day).Distinct().Count()
        //         );

        //     foreach (var teacher in assignments.Select(a => a.Teacher).Distinct())
        //     {
        //         int days = teacherDays[teacher.Name];
        //         if (teacher.PreferFewerDays && days > 3) penalty++;
        //         if (!teacher.PreferFewerDays && days < 3) penalty++;
        //     }
        //     return penalty;
        // };

        // 6. Room type preference
        public static readonly Func<List<Assignment>, int> PreferredRoomTypeConstraint = assignments =>
        {
            int violations = 0;

            foreach (var assignment in assignments)
            {
                if ((assignment.ClassType.PreferredRoomType & assignment.Room.Type) == 0)
                    violations++;
            }

            return violations;
        };

        // 7. At least 3 hours a day for students
        public static readonly Func<List<Assignment>, int> StudentsMin3HoursDay = assignments =>
        {
            int violations = 0;

            var groups = assignments
                .SelectMany(a => a.Class.ClassNames.Select(letter => new
                {
                    Key = (a.Subject.Course, Letter: letter, a.TimeSlot.Day),
                    Assignment = a
                }))
                .GroupBy(x => x.Key)
                .Select(g => g.Select(x => x.Assignment).ToList())
                .ToList();

            foreach (var group in groups) if (group.Sum(a => a.TimeSlot.Time) / 60 < 3) violations++;

            return violations;
        };

        // 8. Avoid gaps in continuous classes
        public static readonly Func<List<Assignment>, int> ContinuousClassGaps = assignments =>
        {
            int gaps = 0;

            var groups = assignments
                .GroupBy(a => new { a.Class.Name, a.TimeSlot.Day })
                .Where(g => g.Count() > 1)
                .Select(g => g.OrderBy(a => a.TimeSlot).ToList());

            foreach (var group in groups)
            {
                var allGaps = ConstraintUtils.GetGaps(group);
                
                foreach (var gap in allGaps)
                {
                    gaps += (int) Math.Floor(gap / TimeSpan.FromMinutes(30));
                }
            }

            return gaps;
        };

        // 9. Continuous classes with different teachers
        public static readonly Func<List<Assignment>, int> ContinuousClassDifferentTeachers = assignments =>
        {
            int violations = 0;

            var groups = assignments
                .GroupBy(a => new { a.Class.Name, a.TimeSlot.Day })
                .Where(g => g.Count() > 1)
                .Select(g => g.OrderBy(a => a.TimeSlot).ToList());

            foreach (var group in groups)
            {
                for (int i = 1; i < group.Count; i++)
                {
                    var prev = group[i - 1];
                    var next = group[i];

                    if (prev.TimeSlot.EndTime - next.TimeSlot.StartTime < TimeSpan.FromMinutes(30) && prev.Teacher != next.Teacher)
                    {
                        violations++;
                    }
                }
            }

            return violations;
        };

        // 10. Continuous classes with different rooms
        public static readonly Func<List<Assignment>, int> ContinuousClassDifferentRooms = assignments =>
        {
            int violations = 0;

            var groups = assignments
                .GroupBy(a => new { a.Class.Name, a.TimeSlot.Day })
                .Where(g => g.Count() > 1)
                .Select(g => g.OrderBy(a => a.TimeSlot).ToList());

            foreach (var group in groups)
            {
                for (int i = 1; i < group.Count; i++)
                {
                    var prev = group[i - 1];
                    var next = group[i];

                    if (prev.TimeSlot.EndTime - next.TimeSlot.StartTime < TimeSpan.FromMinutes(30) && !prev.Room.Equals(next.Room))
                    {
                        violations++;
                    }
                }
            }

            return violations;
        };
        
        // 11. Classes with multiple assignments in the same time slot
        public static readonly Func<List<Assignment>, int> MultipleAssignmentsInSameTimeSlotForClasses = assignments =>
        {
            int violations = 0;

            var groups = assignments
                .SelectMany(a => a.Class.ClassNames.Select(letter => new
                {
                    Key = (a.Subject.Course, Letter: letter, a.TimeSlot),
                    Assignment = a
                }))
                .GroupBy(a => a.Key)
                .Where(g => g.Count() > 1);

            foreach (var group in groups)
            {
                violations += group.Count() - 1; // Count all but one as violations
            }

            return violations;
        };
    }
}
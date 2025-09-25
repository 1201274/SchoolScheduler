using SchoolScheduler.Models;
using SchoolScheduler.Utils;

namespace SchoolScheduler.Core.Constraints
{
    public class HardConstraints
    {
        public static List<Constraint> GetNamedConstraints()
        {
            return new List<Constraint>
            {
                new Constraint { Name = ConstraintName.AssignmentLoadViolation, Func = AssignmentLoadViolation },
                new Constraint { Name = ConstraintName.TeacherConflict, Func = TeacherConflict },
                new Constraint { Name = ConstraintName.StudentsLunchBreak, Func = StudentsLunchBreak },
                new Constraint { Name = ConstraintName.StudentsDinnerBreak, Func = StudentsDinnerBreak },
                new Constraint { Name = ConstraintName.TeachersLunchBreak, Func = TeachersLunchBreak },
                new Constraint { Name = ConstraintName.TeachersDinnerBreak, Func = TeachersDinnerBreak },
                new Constraint { Name = ConstraintName.TeachersMax8HoursDay, Func = TeachersMax8HoursDay },
                new Constraint { Name = ConstraintName.StudentsMax8HoursDay, Func = StudentsMax8HoursDay },
                new Constraint { Name = ConstraintName.RoomCapacityExceeded, Func = RoomCapacityExceeded },
                new Constraint { Name = ConstraintName.RoomConflict, Func = RoomConflict },
            };
        }

        public static List<Delegate> GetConstraints()
        {
            return GetNamedConstraints().Select(c => c.Func).ToList();
        }

        // 1. Teacher cannot be in two places at once
        public static readonly Func<List<Assignment>, int> TeacherConflict = assignments =>
            assignments
                .GroupBy(a => new { a.TimeSlot, a.Teacher })
                .Sum(g => g.Count() > 1 ? g.Count() - 1 : 0);

        // 2. Room cannot be used for more than one class at the same time
        public static readonly Func<List<Assignment>, int> RoomConflict = assignments =>
            assignments
                .GroupBy(a => new { a.TimeSlot, a.Room.Name })
                .Sum(g => g.Count() > 1 ? g.Count() - 1 : 0);

        // 3. Room capacity exceeded
        public static readonly Func<List<Assignment>, int> RoomCapacityExceeded = assignments =>
            assignments.Count(a => a.Class.Students > a.Room.Capacity);

        // 4. Students with classes in both morning and afternoon require lunch break
        public static readonly Func<List<Assignment>, int> StudentsLunchBreak = assignments =>
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

            foreach (var group in groups)
            {
                var hasMorning = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.MORNING);
                var hasAfternoon = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.AFTERNOON);

                if (!hasMorning || !hasAfternoon)
                    continue;

                ConstraintUtils.GetMaxGap(group, Constants.LunchStartTime, Constants.LunchEndTime, out TimeSpan maxGap);

                if (maxGap < Constants.MinMealBreakDuration)
                    violations++;
            }

            return violations;
        };


        // 5. Students with classes after 21h require dinner break
        public static readonly Func<List<Assignment>, int> StudentsDinnerBreak = assignments =>
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

            foreach (var group in groups)
            {
                var hasEvening = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.EVENING);
                var hasNight = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.NIGHT);

                if (!hasEvening || !hasNight)
                    continue;

                ConstraintUtils.GetMaxGap(group, Constants.DinnerStartTime, Constants.DinnerEndTime, out TimeSpan maxGap);

                if (maxGap < Constants.MinMealBreakDuration)
                    violations++;
            }

            return violations;
        };

        // 6. Teachers with classes in both morning and afternoon require lunch break
        public static readonly Func<List<Assignment>, int> TeachersLunchBreak = assignments =>
        {
            int violations = 0;

            var groups = assignments.GroupBy(a => new { a.Teacher, a.TimeSlot.Day }).Select(x => x.ToList()).ToList();

            foreach (var group in groups)
            {
                var hasMorning = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.MORNING);
                var hasAfternoon = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.AFTERNOON);

                if (!hasMorning || !hasAfternoon)
                    continue;

                ConstraintUtils.GetMaxGap(group, Constants.LunchStartTime, Constants.LunchEndTime, out TimeSpan maxGap);

                if (maxGap < Constants.MinMealBreakDuration)
                    violations++;
            }

            return violations;
        };


        // 7. Teachers with classes after 21h require dinner break
        public static readonly Func<List<Assignment>, int> TeachersDinnerBreak = assignments =>
        {
            int violations = 0;

            var groups = assignments.GroupBy(a => new { a.Teacher, a.TimeSlot.Day }).Select(x => x.ToList()).ToList();

            foreach (var group in groups)
            {
                var hasEvening = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.EVENING);
                var hasNight = group.Any(a => a.TimeSlot.TimeOfDay == TimeOfDay.NIGHT);

                if (!hasEvening || !hasNight)
                    continue;

                ConstraintUtils.GetMaxGap(group, Constants.DinnerStartTime, Constants.DinnerEndTime, out TimeSpan maxGap);

                if (maxGap < Constants.MinMealBreakDuration)
                    violations++;
            }

            return violations;
        };

        // 8. No more than 8 hours a day for teachers
        public static readonly Func<List<Assignment>, int> TeachersMax8HoursDay = assignments =>
        {
            int violations = 0;

            var groups = assignments.GroupBy(a => new { a.Teacher, a.TimeSlot.Day });

            foreach (var group in groups) if (group.Sum(a => a.TimeSlot.Time) / 60 > 8) violations++;

            return violations;
        };

        // 9. No more than 8 hours a day for students
        public static readonly Func<List<Assignment>, int> StudentsMax8HoursDay = assignments =>
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

            foreach (var group in groups) if (group.Sum(a => a.TimeSlot.Time) / 60 > 8) violations++;

            return violations;
        };

        // 10. Check if all curriculum is scheduled
        public static readonly Func<List<Assignment>, List<Subject>, int> AssignmentLoadViolation = (assignments, allSubjects) =>
        {
            int underAssigned = 0;
            int overAssigned = 0;

            // Expected load by (Subject, ClassType, Class)
            var expectedLoads = allSubjects
                .SelectMany(subject => subject.ClassTypes
                    .SelectMany(classType => classType.Classes.Select(cls =>
                        new
                        {
                            Key = (Subject: subject, ClassType: classType, Class: cls),
                            Load = classType.TimeSlotsPerWeek
                        })))
                .ToDictionary(e => e.Key, e => e.Load);

            // Actual assignments grouped by same key
            var actualLoads = assignments
                .GroupBy(a => (a.Subject, a.ClassType, a.Class))
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var expected in expectedLoads)
            {
                actualLoads.TryGetValue(expected.Key, out int actual);
                int delta = actual - expected.Value;

                if (delta > 0)
                    overAssigned += delta;
                else if (delta < 0)
                    underAssigned -= delta;
            }

            return Math.Max(overAssigned, underAssigned);
        };
    }

}
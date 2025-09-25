using SchoolScheduler.Core.Constraints;
using SchoolScheduler.Models;
using SchoolScheduler.Utils;

namespace SchoolScheduler.Core
{
    public class AssignmentStatistics
    {
        private readonly List<Assignment> _assignments;
        private readonly SetupService _setup;
        private readonly FitnessEvaluator _evaluator;

        public AssignmentStatistics(List<Assignment> assignments, SetupService setup, FitnessEvaluator evaluator)
        {
            _assignments = assignments;
            _setup = setup;
            _evaluator = evaluator;
        }

        // Statistic fields
        public Dictionary<Constraint, int> ConstraintViolations { get; private set; }
        public double RoomInefficiencyValue { get; private set; }
        public double AverageStudentGapsValue { get; private set; }
        public double AverageHoursPerDayPerClassValue { get; private set; }

        // 1. Room inefficiency: percentage of unused room-days
        public void PrintRoomInefficiency(double? compareValue = null)
        {
            int totalPossibleSlots = _setup.Rooms.Count * _setup.TimeSlots.Count;
            var usedSlots = _assignments
                .Select(a => new { a.Room, a.TimeSlot })
                .Distinct()
                .Count();
            double inefficiency = (double)usedSlots / totalPossibleSlots;
            RoomInefficiencyValue = inefficiency;
            if (compareValue.HasValue)
            {
                double delta = inefficiency - compareValue.Value;
                Console.WriteLine($"Percentage of room use: {inefficiency:P1} ({(delta >= 0 ? "+" : "")}{delta:P1})");
            }
            else
            {
                Console.WriteLine($"Percentage of room use: {inefficiency:P1}");
            }
        }


        // 2. Average student gaps (uses gap constraint function)
        public void PrintAverageStudentGaps(double? compareValue = null)
        {
            var classGroups = _assignments
                .SelectMany(a => a.Class.ClassNames.Select(letter => (
                    a.Subject.Course,
                    Letter: letter,
                    a.TimeSlot.Day
                )))
                .Distinct()
                .Count();

            int totalGaps = SoftConstraints.StudentGaps(_assignments);
            double avg = classGroups > 0 ? (double)totalGaps / classGroups : 0;
            AverageStudentGapsValue = avg * 0.5; // Convert to hours (30 minutes per gap)
            if (compareValue.HasValue)
            {
                double delta = avg - compareValue.Value;
                Console.WriteLine($"Average student gaps: {avg:F2} ({(delta >= 0 ? "+" : "")}{delta:F2})");
            }
            else
            {
                Console.WriteLine($"Average student gaps: {avg:F2}");
            }
        }

        // 3. Average number of hours per day per class
        public void PrintAverageHoursPerDayPerClass(double? compareValue = null)
        {
            var classDayGroups = _assignments
                .SelectMany(a => a.Class.ClassNames.Select(letter => new
                {
                    Key = (a.Subject.Course, Letter: letter, a.TimeSlot.Day),
                    Assignment = a
                }))
                .GroupBy(x => x.Key)
                .Select(g => g.Select(x => x.Assignment).ToList())
                .ToList();


            var minutsPerDay = classDayGroups
                .Select(g => g.Sum(a => a.TimeSlot.Time));

            double avg = minutsPerDay.Any() ? minutsPerDay.Average() / 60 : 0;
            AverageHoursPerDayPerClassValue = avg;
            if (compareValue.HasValue)
            {
                double delta = avg - compareValue.Value;
                Console.WriteLine($"Average hours per day for students: {avg:F2} ({(delta >= 0 ? "+" : "")}{delta:F2})");
            }
            else
            {
                Console.WriteLine($"Average hours/day/class: {avg:F2}");
            }
        }

        public void PrintConstraintViolations(Dictionary<Constraint, int>? compareValue = null)
        {
            ConstraintViolations = _evaluator.NameConstraintsViolated(_assignments);
            var violated = ConstraintViolations
                .Where(c => c.Value > 0)
                .OrderByDescending(c => c.Value)
                .ToList();
            int total = violated.Sum(a => a.Value);
            if (compareValue != null)
            {
                int prevTotal = compareValue.Values.Sum();
                int delta = total - prevTotal;
                Console.WriteLine($"Constraint Violations Summary: {total} ({(delta >= 0 ? "+" : "")}{delta})");
                // Union of all constraint keys
                var allConstraints = violated.Select(v => v.Key).Union(compareValue.Keys).ToList();
                foreach (var constraint in allConstraints)
                {
                    int count = ConstraintViolations.ContainsKey(constraint) ? ConstraintViolations[constraint] : 0;
                    int prev = compareValue.ContainsKey(constraint) ? compareValue[constraint] : 0;
                    int d = count - prev;
                    if (count != 0 || prev != 0)
                    {
                        Console.WriteLine($"\t{constraint.Name.GetDescription()}: {count} ({(d >= 0 ? "+" : "")}{d})");
                    }
                }
            }
            else
            {
                if (!violated.Any())
                {
                    Console.WriteLine("No constraint violations found.");
                    return;
                }
                Console.WriteLine($"Constraint Violations Summary: {total} violations found.");
                foreach (var (constraint, count) in violated)
                {
                    Console.WriteLine($"\t{constraint.Name.GetDescription()}: {count} violation(s)");
                }
            }
        }

        // Print all metrics
        public void PrintSummary(AssignmentStatistics? statisticsToCompare = null)
        {
            Console.WriteLine("==== Assignment Statistics ====");
            if (statisticsToCompare == null)
            {
                PrintConstraintViolations();
                PrintRoomInefficiency();
                PrintAverageStudentGaps();
                PrintAverageHoursPerDayPerClass();
            }
            else
            {
                PrintConstraintViolations(statisticsToCompare.ConstraintViolations);
                PrintRoomInefficiency(statisticsToCompare.RoomInefficiencyValue);
                PrintAverageStudentGaps(statisticsToCompare.AverageStudentGapsValue);
                PrintAverageHoursPerDayPerClass(statisticsToCompare.AverageHoursPerDayPerClassValue);
            }
            Console.WriteLine("================================");
        }
    }

}
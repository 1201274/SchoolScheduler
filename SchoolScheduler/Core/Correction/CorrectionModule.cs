using SchoolScheduler.Core.Constraints;
using SchoolScheduler.Models;
using SchoolScheduler.Utils;

namespace SchoolScheduler.Core.Correction
{
    public class CorrectionModule
    {
        private readonly SetupService _setup;
        private readonly FitnessEvaluator _evaluator;

        private readonly Dictionary<ConstraintName, Delegate> _correctionMethods;

        public CorrectionModule(SetupService setup, FitnessEvaluator evaluator)
        {
            _setup = setup;
            _evaluator = evaluator;

            _correctionMethods = new Dictionary<ConstraintName, Delegate>
            {
                { ConstraintName.AssignmentLoadViolation, AssignmentCorrection.FixAssignmentLoadViolation },
                { ConstraintName.TeacherConflict, TeacherCorrections.FixConflicts },
                { ConstraintName.RoomConflict, RoomCorrections.FixConflicts },
                { ConstraintName.RoomCapacityExceeded, RoomCorrections.FixCapacity },
                { ConstraintName.StudentsLunchBreak, StudentCorrections.FixStudentLunchBreaks },
                { ConstraintName.StudentsDinnerBreak, StudentCorrections.FixStudentDinnerBreaks },
                { ConstraintName.TeachersLunchBreak, TeacherCorrections.FixLunchBreaks },
                { ConstraintName.TeachersDinnerBreak, TeacherCorrections.FixDinnerBreaks },
                { ConstraintName.TeachersMax8HoursDay, TeacherCorrections.FixMaxHoursPerDay },
                { ConstraintName.StudentsMax8HoursDay, StudentCorrections.FixStudentMaxHoursPerDay },
                { ConstraintName.OutOfDepartmentRoomUsage, RoomCorrections.ImproveRoomAssignment },
            };
        }

        public List<Assignment> ApplyCorrections(List<Assignment> assignments, bool progressReport = false)
        {
            var violated = _evaluator.NameConstraintsViolated(assignments);
            int totalViolations = violated.Sum(v => v.Value);
            var reporter = new ProgressReporter(totalViolations);

            int current = 0;

            foreach (var (constraint, count) in violated)
            {
                if (_correctionMethods.TryGetValue(constraint.Name, out var correctionMethod))
                {
                    switch (correctionMethod)
                    {
                        case Action<List<Assignment>> f1: f1(assignments); break;
                        case Action<List<Assignment>, List<TimeSlot>> f2: f2(assignments, _setup.TimeSlots); break;
                        case Action<List<Assignment>, List<Room>> f3: f3(assignments, _setup.Rooms); break;
                        case Action<List<Assignment>, List<Room>, List<TimeSlot>, List<Subject>> f4: f4(assignments, _setup.Rooms, _setup.TimeSlots, _setup.Subjects); break;
                    }
                }

                current += count;
                if (progressReport)
                    reporter.Report(current, $"Fixing constraint: {constraint.Name.GetDescription()}");
            }

            return assignments;
        }
    }
}

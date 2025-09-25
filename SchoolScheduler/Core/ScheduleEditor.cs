using SchoolScheduler.Core.Constraints;
using SchoolScheduler.Models;
using SchoolScheduler.Utils;
using Day = SchoolScheduler.Models.Day;

namespace SchoolScheduler.Core
{
    public class ScheduleEditor
    {
        private Schedule _schedule;
        private readonly FitnessEvaluator _evaluator;
        private readonly SetupService _setup;

        public ScheduleEditor(Schedule schedule)
        {
            _schedule = schedule;

            _setup = new SetupService();
            _setup.Initialize();

            _evaluator = new FitnessEvaluator(_setup);
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("\nSchedule Editor");
                Console.WriteLine("1. Show schedule");
                Console.WriteLine("2. Edit assignment");
                Console.WriteLine("3. Edit all assignments");
                Console.WriteLine("4. Show fitness");
                Console.WriteLine("5. Save schedule");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        ShowSchedule();
                        break;
                    case "2":
                        EditAssignment();
                        break;
                    case "3":
                        EditAllAssignments();
                        break;
                    case "4":
                        UpdateFitness();
                        break;
                    case "5":
                        SaveSchedule();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        private void EditAllAssignments()
        {
            if (_schedule.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments to edit.");
                return;
            }

            for (int i = 0; i < _schedule.Assignments.Count; i++)
            {
                var a = _schedule.Assignments[i];
                Console.WriteLine($"Editing assignment {i + 1}/{_schedule.Assignments.Count} - {a.Class.Name}");
                
                Console.WriteLine($"{i + 1}/{_schedule.Assignments.Count}: {a.Subject?.Name} | {a.Teacher} | {a.Room?.Name} | {a.TimeSlot?.Day} {a.TimeSlot?.StartTime}-{a.TimeSlot?.EndTime} | {a.Class?.Name} | {a.ClassType?.Type}");
                Console.WriteLine($"- Leave blank to continue.");
                Console.WriteLine($"- Type 'copy' copy previous assignment, but next TimeSlot. 'copy n' to copy the next n assignments.");
                Console.WriteLine($"- Type 'skip' to skip the schedule. 'skip n' to skip the next n assignments.");
                Console.WriteLine($"- Type 'save' to save the schedule.");
                Console.WriteLine($"- Type 'exit' to stop editing all assignments.");
                var input = Console.ReadLine()?.Trim().ToLower();
                var command = input?.Split(' ')[0];
                var count = input?.Split(' ').Length > 1 ? int.Parse(input.Split(' ')[1]) : 1;
                if (command == "exit")
                {
                    Console.WriteLine("Exiting assignment editing.");
                    return;
                }
                if (command == "save")
                {
                    SaveSchedule();
                    i--; 
                    continue;
                }
                if (command == "skip")
                {
                    i = Math.Min(i + count - 1, _schedule.Assignments.Count - 1);
                    Console.WriteLine("Skipping assignment.\n");
                    continue;
                }
                if (command == "copy")
                {
                    if (i > 0)
                    {
                        for (int j = 0; j < count && i + j < _schedule.Assignments.Count; count--, i++)
                        {
                            a = _schedule.Assignments[i + j];
                            var previousAssignment = _schedule.Assignments[i + j - 1];
                            if (previousAssignment.TimeSlot == _setup.TimeSlots.Where(ts => ts.Day == previousAssignment.TimeSlot.Day).LastOrDefault())
                            {
                                Console.WriteLine("Cannot copy to next TimeSlot, it is the last of the day.");
                                break;
                            }


                            a.Teacher = previousAssignment.Teacher;
                            a.Room = previousAssignment.Room;
                            a.TimeSlot = _setup.TimeSlots[_setup.TimeSlots.IndexOf(previousAssignment.TimeSlot) + 1];
                            Console.WriteLine($"{i + j + 1}: {a.Subject?.Name} | {a.Teacher} | {a.Room?.Name} | {a.TimeSlot?.Day} {a.TimeSlot?.StartTime}-{a.TimeSlot?.EndTime} | {a.Class?.Name} | {a.ClassType?.Type}\n");
                        }
                        i--;
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("No previous assignment to copy.");
                    }
                }
                EditAssignment(_schedule.Assignments[i]);
            }
        }

        private void UpdateFitness()
        {
            _schedule.Fitness = _evaluator.EvaluateWithWeights(_schedule.Assignments);
            Console.WriteLine($"Fitness: {_schedule.Fitness}\n");

            Dictionary<Constraint, int> ConstraintViolations = _evaluator.NameConstraintsViolated(_schedule.Assignments);
            var violated = ConstraintViolations
                .Where(c => c.Value > 0)
                .OrderByDescending(c => c.Value)
                .ToList();
            int total = violated.Sum(a => a.Value);


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

        private void SaveSchedule()
        {
            try
            {
                _schedule.Fitness = _evaluator.EvaluateWithWeights(_schedule.Assignments);
                JSONScheduleSerializer.ExportSchedule(_schedule, engine: "ManualEdit");
                Console.WriteLine("Schedule saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving schedule: {ex.Message}");
            }
        }

        private void ShowSchedule()
        {
            ScheduleViewer.ShowSchedule(_schedule);
        }

        private void EditAssignment()
        {
            // Filter assignments by partial class name
            if (_schedule.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments to edit.");
                return;
            }

            Console.Write("Class Name: ");
            string className = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(className))
            {
                Console.WriteLine("No class name provided.");
                return;
            }  
            var assignments = _schedule.Assignments
                .Where(a => a.Class?.Name.Contains(className, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            foreach (var a in assignments)
            {
                Console.WriteLine($"{_schedule.Assignments.IndexOf(a)}: {a.Subject?.Name} | {a.Teacher} | {a.Room?.Name} | {a.TimeSlot?.Day} {a.TimeSlot?.StartTime}-{a.TimeSlot?.EndTime} | {a.Class?.Name} | {a.ClassType?.Type}");
            }

            Console.Write("Index to edit: ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 0 && idx < _schedule.Assignments.Count)
            {
                var a = _schedule.Assignments[idx];
                
                Console.WriteLine($"{idx}: {a.Subject?.Name} | {a.Teacher} | {a.Room?.Name} | {a.TimeSlot?.Day} {a.TimeSlot?.StartTime}-{a.TimeSlot?.EndTime} | {a.Class?.Name} | {a.ClassType?.Type}");
                Console.WriteLine("Leave blank to keep current value.\n");
                EditAssignment(a);
            }
            else
            {
                Console.WriteLine("Invalid index.");
            }
        }

        private void EditAssignment(Assignment a)
        {
            while (true)
            {
                Console.Write($"Teacher ({a.Teacher}): ");
                var t = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(t))
                    break;
                if (a.ClassType.TeacherNames.Contains(t))
                {
                    a.Teacher = t;
                    break;
                }
                Console.WriteLine("Teacher does not match ClassType.Teacher.");
                if (a.ClassType.TeacherNames != null && a.ClassType.TeacherNames.Count > 0)
                {
                    Console.WriteLine("Valid teachers for this class type:");
                    foreach (var teacher in a.ClassType.TeacherNames)
                    {
                        Console.WriteLine($"- {teacher}");
                    }
                }
                Console.Write("Try again or leave blank to keep current.\n");
            }

            while (true)
            {
                Console.Write($"Room ({a.Room?.Name}): ");
                var r = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(r))
                    break;
                var room = _setup.Rooms.Find(room => room.Name == r);
                if (room != null)
                {
                    a.Room = room;
                    break;
                }
                Console.WriteLine("Room not found.");
                if (_setup.Rooms != null && _setup.Rooms.Count > 0)
                {
                    Console.WriteLine("Valid rooms:");
                    foreach (var roomItem in _setup.Rooms)
                    {
                        Console.WriteLine($"- {roomItem.Name}");
                    }
                }
                Console.Write("Try again or leave blank to keep current.\n");
            }

            while (true)
            {
                Console.Write($"Day ({a.TimeSlot?.Day}): ");
                var d = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(d))
                    break;
                if (Enum.TryParse<Day>(d, out var day))
                {
                    a.TimeSlot.Day = day;
                    break;
                }
                Console.WriteLine("Day not found.");
                Console.WriteLine("Valid days:");
                foreach (var dayName in Enum.GetNames(typeof(Day)))
                {
                    Console.WriteLine($"- {dayName}");
                }
                Console.Write("Try again or leave blank to keep current.\n");
            }


            while (true)
            {
                Console.Write($"Start ({a.TimeSlot?.StartTime}): ");
                var st = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(st))
                    break;
                if (TimeOnly.TryParse(st, out var startTime))
                {
                    a.TimeSlot = _setup.TimeSlots.Find(ts => ts.StartTime == startTime && ts.Day == a.TimeSlot.Day);
                    break;
                }
                Console.WriteLine("Invalid time format. Please use HH:mm (e.g., 08:30).");
                Console.Write("Try again or leave blank to keep current.\n");
            }
            Console.WriteLine("Assignment updated.\n");
        }
    }
}

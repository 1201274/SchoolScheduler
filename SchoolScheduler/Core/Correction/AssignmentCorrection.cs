using SchoolScheduler.Models;

namespace SchoolScheduler.Core.Correction
{
    public class AssignmentCorrection
    {
        public static void FixAssignmentLoadViolation(List<Assignment> assignments, List<Room> rooms, List<TimeSlot> timeSlots, List<Subject> allSubjects)
        {
            var random = new Random();
            var reusableSlots = new List<(string Teacher, List<char> ClassNames, Course Course, Room Room, TimeSlot TimeSlot)>();
            var newAssignments = new List<Assignment>();

            // Step 1: Expected assignment load by (Subject, ClassType, Class)
            var expectedLoads = allSubjects
                .SelectMany(subject => subject.ClassTypes
                    .SelectMany(classType => classType.Classes.Select(cls =>
                        new
                        {
                            Key = (Subject: subject, ClassType: classType, Class: cls),
                            Load = classType.TimeSlotsPerWeek
                        })))
                .ToList();

            // Step 2: Current assignments grouped by the same key
            var currentAssignments = assignments
                .GroupBy(a => (a.Subject, a.ClassType, a.Class))
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var entry in expectedLoads)
            {
                var key = entry.Key;

                if (!currentAssignments.TryGetValue(key, out var assignedList))
                {
                    assignedList = new List<Assignment>();
                    currentAssignments[key] = assignedList;
                }

                int delta = assignedList.Count - entry.Load;

                if (delta > 0)
                {
                    // Remove excess and track slots
                    var toRemove = assignedList.Skip(entry.Load).ToList();
                    foreach (var assignment in toRemove)
                    {
                        reusableSlots.Add((assignment.Teacher, assignment.Class.ClassNames, assignment.Subject.Course, assignment.Room, assignment.TimeSlot));
                        assignments.Remove(assignment);
                        assignedList.Remove(assignment);
                    }
                }
                else if (delta < 0)
                {
                    // Add missing assignments
                    for (int i = 0; i < -delta; i++)
                    {
                        var newAssignment = new Assignment
                        {
                            Subject = key.Subject,
                            ClassType = key.ClassType,
                            Class = key.Class
                            // Last atributtes are added in following loop
                        };
                        assignments.Add(newAssignment);
                        assignedList.Add(newAssignment);
                        newAssignments.Add(newAssignment);
                    }
                }
            }

            // Step 3: Reuse slot info when possible
            foreach (var assignment in newAssignments)
            {
                if (reusableSlots.Count > 0)
                {
                    var classLetter = assignment.Class.ClassNames.First();
                    var course = assignment.Subject.Course;

                    var classMatch = reusableSlots.Where(slot =>
                        slot.ClassNames.SequenceEqual(assignment.Class.ClassNames) &&
                        slot.Course == course
                    );

                    if (classMatch.Count() > 0)
                    {
                        var match = classMatch.FirstOrDefault(slot =>
                            assignment.ClassType.TeacherNames.Contains(slot.Teacher)
                        );

                        if (match.Equals(default)) match = classMatch.First();

                        assignment.Teacher = match.Teacher;
                        assignment.Room = match.Room;
                        assignment.TimeSlot = match.TimeSlot;
                        reusableSlots.Remove(match);
                    }
                    else
                    {
                        var slot = reusableSlots.First();

                        assignment.Teacher = slot.Teacher;
                        assignment.Room = slot.Room;
                        assignment.TimeSlot = slot.TimeSlot;
                    }
                    
                }
                else
                {
                    // fallback
                    assignment.Teacher = assignment.ClassType.TeacherNames[random.Next(assignment.ClassType.TeacherNames.Count)];
                    assignment.Room = rooms[random.Next(rooms.Count)];
                    assignment.TimeSlot = timeSlots[random.Next(timeSlots.Count)];
                }
            }
        }


    }
}
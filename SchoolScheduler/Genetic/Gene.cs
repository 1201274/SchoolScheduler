using SchoolScheduler.Models;
using SchoolScheduler.Core;

namespace SchoolScheduler.Genetic
{
    public class Gene
    {
        public Schedule Schedule;

        public Gene()
        {}

        public Gene(List<Subject> subjects, List<Room> rooms, List<TimeSlot> slots, Random rand)
        {
            Schedule = new Schedule(subjects, rooms, slots, rand);
        }

        public Gene Clone()
        {
            return new Gene
            {
                Schedule = Schedule.Clone()
            };
        }

        // Crossover: single point
        public static Gene Crossover(Gene parent1, Gene parent2)
        {
            int point = new Random().Next(parent1.Schedule.Assignments.Count);
            var childAssignments = parent1.Schedule.Assignments.Take(point)
                .Concat(parent2.Schedule.Assignments.Skip(point)).Select(a => a.Clone()).ToList();
            return new Gene
            {
                Schedule = new Schedule(childAssignments, int.MaxValue)// Fitness will be evaluated later
            };
        }

        // Mutation: randomly change one assignment
        public void Mutate(SetupService setup)
        {
            if (Schedule.Assignments.Count == 0) return;
            int idx = new Random().Next(Schedule.Assignments.Count);
            var assignment = Schedule.Assignments[idx];
            // Example mutation: change room or timeslot randomly
            assignment.Teacher = assignment.ClassType.TeacherNames[new Random().Next(assignment.ClassType.TeacherNames.Count)];
            assignment.Room = setup.Rooms[new Random().Next(setup.Rooms.Count)];
            assignment.TimeSlot = setup.TimeSlots[new Random().Next(setup.TimeSlots.Count)];
        }
    }
}
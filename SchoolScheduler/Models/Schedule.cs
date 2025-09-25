using SchoolScheduler.Core;

namespace SchoolScheduler.Models
{
    public class Schedule
    {
        public List<Assignment> Assignments { get; set; }
        public int Fitness { get; set; }

        public Schedule()
        {}

        public Schedule(List<Assignment> assignments, int fitness)
        {
            Assignments = [.. assignments];
            Fitness = fitness;
        }

        public Schedule(List<Subject> subjects, List<Room> rooms, List<TimeSlot> slots, Random rand)
        {
            Assignments = new List<Assignment>();

            foreach (var subject in subjects)
            {
                foreach (var classType in subject.ClassTypes)
                {
                    foreach (var className in classType.Classes)
                    {
                        for (int h = 0; h < classType.TimeSlotsPerWeek; h++)
                        {
                            var teacher = classType.TeacherNames[rand.Next(classType.TeacherNames.Count)];
                            var room = rooms[rand.Next(rooms.Count)];
                            var slot = slots[rand.Next(slots.Count)];

                            Assignments.Add(new Assignment
                            {
                                Subject = subject,
                                Teacher = teacher,
                                Room = room,
                                TimeSlot = slot,
                                Class = className,
                                ClassType = classType
                            });
                        }
                    }
                }
            }

            Fitness = int.MaxValue;
        }

        public Schedule Clone()
        {
            return new Schedule(Assignments.Select(a => a.Clone()).ToList(), Fitness);
        }

        
        public void Evaluate(FitnessEvaluator evaluator)
        {
            Fitness = evaluator.EvaluateWithWeights(Assignments);
        }
    }
}
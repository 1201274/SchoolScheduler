using SchoolScheduler.Models;

namespace SchoolScheduler.PSO
{
    public class Particle
    {
        public Schedule Schedule;
        public Schedule BestSchedule;

        public Particle()
        {}

        public Particle(List<Subject> subjects, List<Room> rooms, List<TimeSlot> slots, Random rand)
        {
            Schedule = new Schedule(subjects, rooms, slots, rand);
            BestSchedule = Schedule.Clone();
        }

        public Particle Copy()
        {
            return new Particle
            {
                Schedule = Schedule.Clone(),
                BestSchedule = BestSchedule.Clone()
            };
        }
    }
}
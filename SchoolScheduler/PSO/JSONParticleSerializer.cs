namespace SchoolScheduler.PSO
{
    using SchoolScheduler.Models;
    using SchoolScheduler.Utils;

    public class JSONParticleSerializer
    {
        private static readonly string _defaultDirectory = "Schedules/Schedules";
        private static Func<int, string> _defaultFilePath = (fitness) => $"{_defaultDirectory}/{fitness}_{DateTime.Now:ddMMyyyy_HHmmss}_PSO.json";

        public static Particle ImportParticle(string filePath)
        {
            var schedule = JSONScheduleSerializer.ImportSchedule(filePath);
            return new Particle
            {
                Schedule = schedule,
                BestSchedule = schedule.Clone()
            };
        }

        public static Particle ImportRandomParticle()
        {
            var schedule = JSONScheduleSerializer.ImportRandomSchedule();
            return new Particle
            {
                Schedule = schedule,
                BestSchedule = schedule.Clone()
            };
        }

        public static void ExportParticle(Particle particle)
        {
            JSONScheduleSerializer.ExportSchedule(particle.BestSchedule, _defaultFilePath(particle.BestSchedule.Fitness));
        }
    }
}
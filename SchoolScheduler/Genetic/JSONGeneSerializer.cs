namespace SchoolScheduler.Genetic
{
    using SchoolScheduler.Models;
    using SchoolScheduler.Utils;

    public class JSONGeneSerializer
    {
        private static readonly string _defaultDirectory = "Schedules/Schedules";
        private static Func<int, string> _defaultFilePath = (fitness) => $"{_defaultDirectory}/{fitness}_{DateTime.Now:ddMMyyyy_HHmmss}_Genetic.json";

        public static Gene ImportGene(string filePath)
        {
            var schedule = JSONScheduleSerializer.ImportSchedule(filePath);
            return new Gene
            {
                Schedule = schedule
            };
        }

        public static Gene ImportRandomGene()
        {
            var schedule = JSONScheduleSerializer.ImportRandomSchedule();
            return new Gene
            {
                Schedule = schedule
            };
        }

        public static void ExportGene(Gene gene)
        {
            JSONScheduleSerializer.ExportSchedule(gene.Schedule, _defaultFilePath(gene.Schedule.Fitness));
        }
    }
}
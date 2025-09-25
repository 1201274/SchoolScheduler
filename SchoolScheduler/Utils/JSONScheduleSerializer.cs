namespace SchoolScheduler.Utils
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using SchoolScheduler.Models;

    public class JSONScheduleSerializer
    {
        private static readonly string _defaultDirectory = "Schedules/Schedules";
        private static Func<int, string, string> _defaultFilePath = (fitness, engine) => $"{_defaultDirectory}/{fitness}_{DateTime.Now:ddMMyyyy_HHmmss}{(!string.IsNullOrEmpty(engine) ? $"_{engine}" : "")}.json";

        public static void ExportSchedule(Schedule schedule, string filePath = null, string engine = null)
        {
            FileInfo fileInfo = new FileInfo(filePath ?? _defaultFilePath(schedule.Fitness, engine));
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
            var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true, Converters = { new JsonStringEnumConverter() } };
            var json = JsonSerializer.Serialize(schedule, options);
            File.WriteAllText(fileInfo.FullName, json);
        }

        public static Schedule ImportSchedule(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File {filePath} does not exist.");
            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { IncludeFields = true, PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };
            var dto = JsonSerializer.Deserialize<Schedule>(json, options);
            return new Schedule(dto.Assignments, dto.Fitness);
        }

        public static Schedule ImportRandomSchedule()
        {
            if (!Directory.Exists(_defaultDirectory))
                throw new DirectoryNotFoundException($"Directory {_defaultDirectory} does not exist. No schedule imported.");

            var files = Directory.GetFiles(_defaultDirectory, "*.json");
            if (files.Length == 0)
                throw new FileNotFoundException($"No schedules found in {_defaultDirectory} directory to import.");

            var random = new Random();
            var randomSchedule = files[random.Next(files.Length)];

            return ImportSchedule(randomSchedule);
        }
    }
}
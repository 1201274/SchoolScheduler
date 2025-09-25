using System.Text.Json;
using System.Text.Json.Serialization;
using SchoolScheduler.Models;

namespace SchoolScheduler.Core
{
    public class ScheduleViewer
    {
        public static void ShowSchedule(Schedule schedule)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true, Converters = { new JsonStringEnumConverter() } };
                var json = System.Text.Json.JsonSerializer.Serialize(schedule, options);
                var htmlTemplatePath = System.IO.Path.GetFullPath("Schedules/schedule-viewer.html");
                var htmlContent = System.IO.File.ReadAllText(htmlTemplatePath);
                // Replace CSS path with absolute path
                var cssAbsolutePath = System.IO.Path.GetFullPath("Schedules/schedule-viewer.css");
                htmlContent = htmlContent.Replace("href=\"schedule-viewer.css\"", $"href=\"{cssAbsolutePath}\"");
                // Insert schedule JSON as a global variable at the top of the <script> block
                var scriptTag = "schedule = " + json + ";\n render()\n";
                int scriptIndex = htmlContent.IndexOf("</script>");
                if (scriptIndex >= 0)
                {
                    htmlContent = htmlContent.Substring(0, scriptIndex) + scriptTag + htmlContent.Substring(scriptIndex);
                }
                else
                {
                    // fallback: prepend script
                    htmlContent = scriptTag + htmlContent;
                }
                var tempHtmlPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"schedule_{Guid.NewGuid()}.html");
                System.IO.File.WriteAllText(tempHtmlPath, htmlContent);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempHtmlPath,
                    UseShellExecute = true
                });
                Console.WriteLine($"Opened schedule viewer with current schedule.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not open schedule viewer: {ex.Message}");
            }
        }
    }
}
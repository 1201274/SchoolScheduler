using SchoolScheduler.Core;
using SchoolScheduler.Core.Correction;
using SchoolScheduler.Genetic;
using SchoolScheduler.Models;
using SchoolScheduler.PSO;
using SchoolScheduler.Utils;
using System.Windows.Forms;

namespace SchoolScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\nMain Menu");
                Console.WriteLine("1. Create new schedule");
                Console.WriteLine("2. Modify existing schedule");
                Console.WriteLine("3. Apply correction module to schedule");
                Console.WriteLine("4. Generate graphs for algorithms");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        new Scheduler().Run();
                        break;
                    case "2":
                        ModifyExistingSchedule();
                        break;
                    case "3":
                        ApplyCorrectionModuleToSchedule();
                        break;
                    case "4":
                        GraphGenerator.GenerateGraphs();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        static Schedule GetSchedule()
        {
            string selectedFile = null;
            var thread = new System.Threading.Thread(() => {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.InitialDirectory = Path.GetFullPath("Schedules/Schedules");
                    dialog.Filter = "JSON files (*.json)|*.json";
                    dialog.Title = "Select a schedule file";
                    if (dialog.ShowDialog() == DialogResult.OK)
                        selectedFile = dialog.FileName;
                }
            });
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (string.IsNullOrEmpty(selectedFile))
            {
                Console.WriteLine("No file selected.");
                return null;
            }
            return JSONScheduleSerializer.ImportSchedule(selectedFile);
        }

        static void ModifyExistingSchedule()
        {
            var schedule = GetSchedule();
            if (schedule != null)
                new ScheduleEditor(schedule).Run();

        }
        
        static void ApplyCorrectionModuleToSchedule()
        {
            var schedule = GetSchedule();
            if (schedule != null)
            {
                var setup = new SetupService();
                setup.Initialize();
                var evaluator = new FitnessEvaluator(setup);
                var correctionModule = new CorrectionModule(setup, evaluator);

                var originalStatistics = new AssignmentStatistics(schedule.Assignments, setup, evaluator);
                schedule.Fitness = evaluator.EvaluateWithWeights(schedule.Assignments);
                Console.WriteLine($"Original Schedule Statistics:\nSchedule (Fitness: {schedule.Fitness})\n--------------------");
                originalStatistics.PrintSummary();

                var correctedAssignments = correctionModule.ApplyCorrections(schedule.Assignments, progressReport: true);

                var correctedFitness = evaluator.EvaluateWithWeights(correctedAssignments);

                var correctedStatistics = new AssignmentStatistics(correctedAssignments, setup, evaluator);

                var fitnessDelta = correctedFitness - schedule.Fitness;
                Console.WriteLine($"\nCorrected Schedule (Fitness: {correctedFitness} ({(fitnessDelta >= 0 ? "+" : "")}{fitnessDelta}))\n--------------------");

                correctedStatistics.PrintSummary(originalStatistics);
                
                if (correctedFitness < schedule.Fitness)
                    JSONScheduleSerializer.ExportSchedule(new Schedule(correctedAssignments, correctedFitness), engine: $"_Corrected");
                
            }
        }
    }
}
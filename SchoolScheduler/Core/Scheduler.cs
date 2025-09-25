using SchoolScheduler.Core.Correction;
using SchoolScheduler.Genetic;
using SchoolScheduler.Models;
using SchoolScheduler.PSO;
using SchoolScheduler.Utils;

namespace SchoolScheduler.Core
{
    public class Scheduler
    {
        public const int DefaultPopulationSize = 100;
        public const int DefaultGenerations = 50;
        public const int DefaultSwarmSize = 100;
        public const int DefaultIterations = 100;
        public const int DefaultImported = 10;


        public void Run()
        {
            var setup = new SetupService();
            setup.Initialize();
            var evaluator = new FitnessEvaluator(setup);
            var correctionModule = new CorrectionModule(setup, evaluator);
            Schedule best = null!;


            Console.WriteLine("Choose Algorithm:");
            Console.WriteLine("1. PSO");
            Console.WriteLine("2. PSO with Correction Module");
            Console.WriteLine("3. Genetic");
            Console.WriteLine("4. Genetic with Correction Module");
            Console.Write("Engine: ");
            var engineInput = Console.ReadLine();
            string engine = "";
            bool useCorrectionModule = false;
            int popSize = 0, imported = 0, iters = 0, gens = 0;

            switch (engineInput)
            {
                case "1":
                    engine = $"PSO{(useCorrectionModule ? "_Correction" : "")}";
                    Console.Write($"Swarm size ({DefaultSwarmSize}): ");
                    int.TryParse(Console.ReadLine(), out popSize);
                    popSize = popSize == 0 ? DefaultSwarmSize : popSize;
                    Console.Write($"Iterations ({DefaultIterations}): ");
                    int.TryParse(Console.ReadLine(), out iters);
                    iters = iters == 0 ? DefaultIterations : iters;
                    Console.Write($"Imported individuals ({DefaultImported}): ");
                    int.TryParse(Console.ReadLine(), out imported);
                    imported = imported == 0 ? DefaultImported : imported;


                    var pso = new PSOEngine(setup, evaluator, correctionModule);
                    best = pso.Execute(swarmSize: popSize, iterations: iters, nImportedParticles: imported);

                    break;
                case "2":
                    useCorrectionModule = true;
                    goto case "1";
                case "3":
                    engine = $"Genetic{(useCorrectionModule ? "_Correction" : "")}";
                    Console.Write($"Population size ({DefaultPopulationSize}): ");
                    int.TryParse(Console.ReadLine(), out popSize);
                    popSize = popSize == 0 ? DefaultPopulationSize : popSize;
                    Console.Write($"Generations ({DefaultGenerations}): ");
                    int.TryParse(Console.ReadLine(), out gens);
                    gens = gens == 0 ? DefaultGenerations : gens;
                    Console.Write($"Imported individuals ({DefaultImported}): ");
                    int.TryParse(Console.ReadLine(), out imported);
                    imported = imported == 0 ? DefaultImported : imported;

                    var ga = new GeneticEngine(setup, evaluator, correctionModule);
                    best = ga.Run(populationSize: popSize, generations: gens, nImportedIndividuals: imported);

                    break;
                case "4":
                    useCorrectionModule = true;
                    goto case "3";
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            JSONScheduleSerializer.ExportSchedule(best, engine: engine);

            var bestStatistics = new AssignmentStatistics(best.Assignments, setup, evaluator);
            Console.WriteLine($"\n\nBest Schedule (Fitness: {best.Fitness})\n--------------------");
            bestStatistics.PrintSummary();
            
            ScheduleViewer.ShowSchedule(best);
        }
    }
}
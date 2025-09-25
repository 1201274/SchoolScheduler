using SchoolScheduler.Models;
using SchoolScheduler.Core;
using SchoolScheduler.Core.Correction;
using SchoolScheduler.Utils;

namespace SchoolScheduler.Genetic
{
    public class GeneticEngine
    {
        private readonly SetupService _setup;
        private readonly FitnessEvaluator _evaluator;
        private readonly CorrectionModule _correctionModule;
        private readonly Random _random = new();
        public GeneticEngine(SetupService setup, FitnessEvaluator evaluator, CorrectionModule correctionModule)
        {
            _setup = setup;
            _evaluator = evaluator;
            _correctionModule = correctionModule;
        }

        public Schedule Run(int populationSize = 30, int generations = 100, double mutationRate = 0.2, int nImportedIndividuals = 10, bool useCorrectionModule = true)
        {
            return Run(out _, populationSize, generations, mutationRate, nImportedIndividuals, useCorrectionModule);
        }

        public Schedule Run(out List<GraphPoint> stats, int populationSize = 30, int generations = 100, double mutationRate = 0.2, int nImportedIndividuals = 10, bool useCorrectionModule = true)
        {
            var initialPopulation = new List<Gene>();

            try
            {
                for (int i = 0; i < nImportedIndividuals; i++)
                {
                    initialPopulation.Add(JSONGeneSerializer.ImportRandomGene());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GeneticEngine] Import: {ex.Message}");
            }
            
            return Run(out stats, populationSize, generations, mutationRate, initialPopulation, useCorrectionModule);
        }
        public Schedule Run(out List<GraphPoint> stats, int populationSize = 30, int generations = 100, double mutationRate = 0.2, List<Gene> initialPopulation = null, bool useCorrectionModule = true, bool progressReporting = true)
        {
            stats = new List<GraphPoint>();
            var population = new List<Gene>();

            initialPopulation ??= new List<Gene>();
            population.AddRange(initialPopulation);

            var random = new Random();
            for (int i = population.Count; i < populationSize; i++)
            {
                population.Add(new Gene(_setup.Subjects, _setup.Rooms, _setup.TimeSlots, _random));
            }
            foreach (var gene in population) gene.Schedule.Evaluate(_evaluator);
            var reporter = new ProgressReporter(generations);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int gen = 0; gen < generations; gen++)
            {
                // Variation: mutation and correction
                foreach (var gene in population)
                {
                    if (random.NextDouble() < mutationRate)
                        gene.Mutate(_setup);
                    if (useCorrectionModule && random.NextDouble() < 0.001)
                        gene.Schedule.Assignments = _correctionModule.ApplyCorrections(gene.Schedule.Assignments, progressReport: progressReporting);
                    gene.Schedule.Evaluate(_evaluator);
                }
                // Selection: top 50%
                population = population.OrderBy(g => g.Schedule.Fitness).Take(populationSize / 2).ToList();
                // Crossover and mutation to refill population
                while (population.Count < populationSize)
                {
                    var parents = population.OrderBy(_ => Guid.NewGuid()).Take(2).ToList();
                    var child = Gene.Crossover(parents[0], parents[1]);
                    if (random.NextDouble() < mutationRate) child.Mutate(_setup);
                    child.Schedule.Evaluate(_evaluator);
                    population.Add(child);
                }
                var bestFitness = population[0].Schedule.Fitness;
                var meanFitness = population.Average(g => g.Schedule.Fitness);
                var time = sw.Elapsed.TotalSeconds;
                stats.Add(new GraphPoint(time, gen, bestFitness, meanFitness));
                if (progressReporting)
                    reporter.Report(gen, $"Global Best Fitness:{bestFitness} | Mean Fitness: {meanFitness}");
            }
            return new Schedule(population[0].Schedule.Assignments, population[0].Schedule.Fitness);
        }
    }
}
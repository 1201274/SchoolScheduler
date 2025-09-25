using SchoolScheduler.Core;
using SchoolScheduler.Core.Correction;
using SchoolScheduler.Genetic;
using SchoolScheduler.Models;
using SchoolScheduler.PSO;

namespace SchoolScheduler.Utils
{
    public class GraphGenerator
    {
        public static void GenerateGraphs()
        {
            var setup = new SetupService();
            setup.Initialize();
            var evaluator = new FitnessEvaluator(setup);
            var correctionModule = new CorrectionModule(setup, evaluator);

            var pso = new PSOEngine(setup, evaluator, correctionModule);
            var ga = new GeneticEngine(setup, evaluator, correctionModule);

            int[] swarmSizes = { 30, 100 };
            int iterations = 2000;
            int generations = 1000;

            // var LEI = JSONScheduleSerializer.ImportSchedule("Schedules/LEI_2024_25.json");
            var LEI = new Schedule(setup.Subjects, setup.Rooms, setup.TimeSlots, new Random(42));
            var progressReporter = new ProgressReporter(swarmSizes.Length * 12 * 5); // total runs

            int progressIndex = 0;

            foreach (var size in swarmSizes)
            {
                var configurations = new List<(string Key, Func<List<Gene>, List<Particle>, (Schedule schedule, List<GraphPoint> stats)> Run)>
                {
                    ($"GA_{size}", (genes, particles) => (ga.Run(out var stats, size, generations, 0.2, null, false, true), stats)),
                    ($"GA_{size}_Correction", (genes, particles) => (ga.Run(out var stats, size, generations, 0.2, null, true, true), stats)),
                    ($"PSO_{size}", (genes, particles) => (pso.Execute(out var stats, size, iterations, null, false, true), stats)),
                    ($"PSO_{size}_Correction", (genes, particles) => (pso.Execute(out var stats, size, iterations, null, true, true), stats)),
                    ($"GA_{size}_Initial", (genes, particles) => (ga.Run(out var stats, size, generations, 0.2, genes, false, true), stats)),
                    ($"GA_{size}_Initial_Correction", (genes, particles) => (ga.Run(out var stats, size, generations, 0.2, genes, true, true), stats)),
                    ($"PSO_{size}_Initial", (genes, particles) => (pso.Execute(out var stats, size, iterations, particles, false, true), stats)),
                    ($"PSO_{size}_Initial_Correction", (genes, particles) => (pso.Execute(out var stats, size, iterations, particles, true, true), stats)),
                    ($"GA_{size}_Initial_LEI", (genes, particles) => (ga.Run(out var stats, size, generations, 0.2, genes, false, true), stats)),
                    ($"GA_{size}_Initial_LEI_Correction", (genes, particles) => (ga.Run(out var stats, size, generations, 0.2, genes, true, true), stats)),
                    ($"PSO_{size}_Initial_LEI", (genes, particles) => (pso.Execute(out var stats, size, iterations, particles, false, true), stats)),
                    ($"PSO_{size}_Initial_LEI_Correction", (genes, particles) => (pso.Execute(out var stats, size, iterations, particles, true, true), stats)),
                };

                var initialGenes = new List<Gene>();
                var initialParticles = new List<Particle>();

                foreach (var (key, runMethod) in configurations)
                {
                    var allStats = new List<GraphPoint>();

                    if (File.Exists($"Graphs/{key}.csv"))
                    {
                        // If the CSV already exists, skip this run
                        progressIndex += 5; // Skip 5 runs for this key
                        Console.WriteLine($"Skipping {key} as it already exists.");

                        // Add initial genes and particles generated when the CSV was created
                        if (!key.Contains("Initial"))
                            for (int i = 1; i <= 5; i++)
                            {
                                var existingStats = JSONScheduleSerializer.ImportSchedule($"Graphs/Schedules/{key}_{i}.json");
                                initialGenes.Add(new Gene { Schedule = existingStats });
                                initialParticles.Add(new Particle { Schedule = existingStats, BestSchedule = existingStats.Clone() });
                            }

                        continue;
                    }
                    
                    for (int i = 0; i < 5; i++)
                    {
                        List<Gene> genes = null;
                        List<Particle> particles = null;

                        if (key.Contains("Initial"))
                        {
                            genes = new List<Gene>();
                            particles = new List<Particle>();

                            for (int j = i; j < initialGenes.Count; j += 5)
                            {
                                genes.Add(initialGenes[j]);
                                particles.Add(initialParticles[j]);
                            }

                            if (key.Contains("LEI"))
                            {
                                genes.Add(new Gene { Schedule = LEI });
                                particles.Add(new Particle { Schedule = LEI, BestSchedule = LEI.Clone() } );
                            }
                        }


                        var (schedule, stats) = runMethod(genes, particles);
                        allStats.AddRange(stats);


                        // Save these as initial for later "initial" runs
                        if (!key.Contains("Initial"))
                        {
                            JSONScheduleSerializer.ExportSchedule(schedule, $"Graphs/Schedules/{key}_{i + 1}.json");
                            initialGenes.Add(new Gene { Schedule = schedule });
                            initialParticles.Add(new Particle { Schedule = schedule, BestSchedule = schedule.Clone() });
                        }

                        progressReporter.Report(progressIndex++, $"Run {i + 1}/5 | {key} | Last Fitness: {schedule.Fitness}");
                    }

                    // Averaging by generation
                    var averagedStats = allStats
                        .GroupBy(p => p.Generation)
                        .OrderBy(g => g.Key)
                        .Select(g =>
                        {
                            double avgTime = Math.Round(g.Average(p => p.Time), 3);
                            int avgBestFitness = (int)Math.Round(g.Average(p => p.BestFitness));
                            double avgMeanFitness = Math.Round(g.Average(p => p.MeanFitness), 2);
                            return new GraphPoint(avgTime, g.Key, avgBestFitness, avgMeanFitness);
                        })
                        .ToList();

                    ExportStatsToCsv($"Graphs/{key}.csv", averagedStats, key);
                }
            }
        }


        public static void ExportStatsToCsv(string filePath, List<GraphPoint> stats, string algorithmName)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Algorithm;Generation;Time;BestFitness;MeanFitness");
                foreach (var stat in stats)
                {
                    writer.WriteLine($"{algorithmName};{stat.Generation};{stat.Time};{stat.BestFitness};{stat.MeanFitness}");
                }
            }
        }

    }


    public class GraphPoint
    {
        public GraphPoint(double time, int generation, int bestFitness, double meanFitness)
        {
            Time = time;
            Generation = generation;
            BestFitness = bestFitness;
            MeanFitness = meanFitness;
        }

        public double Time { get; set; }
        public int Generation { get; set; }
        public int BestFitness { get; set; }
        public double MeanFitness { get; set; }

        public override string ToString()
        {
            return $"Time: {Time}, Generation: {Generation}, Best Fitness: {BestFitness}, Mean Fitness: {MeanFitness}";
        }
    }
}
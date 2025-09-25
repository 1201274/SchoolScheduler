using SchoolScheduler.Models;
using SchoolScheduler.Utils;
using SchoolScheduler.Core;
using SchoolScheduler.Core.Correction;

namespace SchoolScheduler.PSO
{
    public class PSOEngine
    {
        private readonly SetupService _setup;
        private readonly FitnessEvaluator _evaluator;
        private readonly CorrectionModule _correctionModule;
        private readonly Random _random = new();

        public PSOEngine(SetupService setup, FitnessEvaluator evaluator, CorrectionModule correctionModule)
        {
            _setup = setup;
            _evaluator = evaluator;
            _correctionModule = correctionModule;
        }

        public Schedule Execute(int swarmSize = 30, int iterations = 100000, int nImportedParticles = 10, bool useCorrectionModule = true)
        {
            return Execute(out _, swarmSize, iterations, nImportedParticles, useCorrectionModule);
        }

        public Schedule Execute(out List<GraphPoint> stats, int swarmSize = 30, int iterations = 100000, int nImportedParticles = 10, bool useCorrectionModule = true)
        {
            var initialSwarm = new List<Particle>();
            try
            {
                // Load existing particles from JSON files
                for (int i = 0; i < nImportedParticles; i++)
                {
                    var particle = JSONParticleSerializer.ImportRandomParticle();
                    initialSwarm.Add(particle);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            return Execute(out stats, swarmSize, iterations, initialSwarm, useCorrectionModule);
        }


        public Schedule Execute(out List<GraphPoint> stats, int swarmSize = 30, int iterations = 100000, List<Particle> initialSwarm = null, bool useCorrectionModule = true, bool progressReporting = true)
        {
            stats = new List<GraphPoint>();
            var swarm = new List<Particle>();
            
            initialSwarm ??= new List<Particle>();
            swarm.AddRange(initialSwarm);

            Schedule globalBest = null!;

            for (int i = swarm.Count; i < swarmSize; i++)
            {
                var particle = new Particle(_setup.Subjects, _setup.Rooms, _setup.TimeSlots, _random);
                particle.Schedule.Fitness = _evaluator.EvaluateWithWeights(particle.Schedule.Assignments);
                particle.BestSchedule = particle.Schedule.Clone();

                if (globalBest == null || particle.Schedule.Fitness < globalBest.Fitness)
                    globalBest = particle.BestSchedule.Clone();

                swarm.Add(particle);
            }

            var reporter = new ProgressReporter(iterations);
            var sw = System.Diagnostics.Stopwatch.StartNew();

            for (int iter = 0; iter < iterations; iter++)
            {
                foreach (var particle in swarm)
                {
                    foreach (var assignment in particle.Schedule.Assignments)
                    {

                        if (_random.NextDouble() < 0.1)
                        {
                            assignment.Teacher = assignment.ClassType.TeacherNames[_random.Next(assignment.ClassType.TeacherNames.Count)];
                        }
                        if (_random.NextDouble() < 0.1)
                            assignment.Room = _setup.Rooms[_random.Next(_setup.Rooms.Count)];
                        if (_random.NextDouble() < 0.1)
                            assignment.TimeSlot = _setup.TimeSlots[_random.Next(_setup.TimeSlots.Count)];
                    }

                    if (useCorrectionModule && _random.NextDouble() < 0.001)
                        particle.Schedule.Assignments = _correctionModule.ApplyCorrections(particle.Schedule.Assignments, progressReport: progressReporting);

                    particle.Schedule.Evaluate(_evaluator);

                    if (particle.Schedule.Fitness < particle.BestSchedule.Fitness)
                    {
                        particle.BestSchedule = particle.Schedule.Clone();

                    if (globalBest == null || particle.Schedule.Fitness < globalBest.Fitness)
                        globalBest = particle.BestSchedule.Clone();

                    }
                }


                var time = sw.Elapsed.TotalSeconds;
                var meanFitness = swarm.Average(g => g.BestSchedule.Fitness);

                stats.Add(new GraphPoint(time, iter, globalBest.Fitness, meanFitness));
                if (progressReporting)
                    reporter.Report(iter, $"Global Best Fitness:{globalBest.Fitness} | Mean Fitness: {meanFitness}");
            }

            return globalBest;
        }
    }
}

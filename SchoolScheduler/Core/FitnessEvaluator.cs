using SchoolScheduler.Core.Constraints;
using SchoolScheduler.Models;

namespace SchoolScheduler.Core
{
    public class FitnessEvaluator
    {
        private readonly SetupService _setup;

        public FitnessEvaluator( SetupService setup)
        {
            _setup = setup;
        }

        public int CountViolatedConstraints(List<Assignment> assignments)
        {
            return NameConstraintsViolated(assignments).Values.Sum();
        }

        public int EvaluateWithWeights(List<Assignment> assignments)
        {
            return NameConstraintsViolated(assignments).Sum(kv => kv.Key.Name.GetWeight() * kv.Value);
        }

        public Dictionary<Constraint, int> NameConstraintsViolated(List<Assignment> assignments)
        {
            var result = new Dictionary<Constraint, int>();
            foreach (var constraint in _setup.Constraints)
            {
                int violations = constraint.Func switch
                {
                    Func<List<Assignment>, int> f1 => f1(assignments),
                    Func<List<Assignment>, List<Subject>, int> f2 => f2(assignments, _setup.Subjects),
                    _ => 0
                };
                result[constraint] = violations;
            }
            return result;
        }
    }
}
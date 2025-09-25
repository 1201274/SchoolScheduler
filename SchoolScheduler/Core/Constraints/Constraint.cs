namespace SchoolScheduler.Core.Constraints
{
    public class Constraint
    {
        public ConstraintName Name { get; set; }
        public Delegate Func { get; set; }
    }
}
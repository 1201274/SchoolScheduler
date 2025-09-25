using System;
using System.Collections.Generic;
using System.Linq;
using SchoolScheduler.Core.Constraints;
using SchoolScheduler.Models;

namespace SchoolScheduler.Core.Initializers
{
    public class ConstraintInitializer
    {

        public List<Constraint> Initialize()
        {
            var constraints = new List<Constraint>();
            constraints.AddRange(HardConstraints.GetNamedConstraints());
            constraints.AddRange(SoftConstraints.GetNamedConstraints());
            return constraints;
        }
    }
}

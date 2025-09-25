using System.Collections.Generic;

namespace SchoolScheduler.Models
{
    public class Teacher
    {
        public string Name { get; set; }
        public List<string> QualifiedSubjects { get; set; }
        public bool MobilityReduced { get; set; } = false;
        public List<string> PreferredDays { get; set; } = new();
        public List<string> AvoidEnglishGroups { get; set; } = new();

        public Teacher(string name, List<string> subjects)
        {
            Name = name;
            QualifiedSubjects = subjects;
        }
    }
}
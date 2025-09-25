namespace SchoolScheduler.Models
{
    public class Course
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public Department Department { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Course other)
            {
                return Name == other.Name && Year == other.Year && Department.Equals(other.Department);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = Name?.GetHashCode() ?? 0;
            hash = (hash * 397) ^ Year.GetHashCode();
            hash = (hash * 397) ^ Department.GetHashCode();
            return hash;
        }    
    }
}
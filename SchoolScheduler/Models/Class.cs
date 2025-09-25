namespace SchoolScheduler.Models
{
    public class Class
    {
        public string Name { get; set; }
        public int Students { get; set; }
        public List<char> ClassNames { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Class other)
            {
                return Name == other.Name;
                // return Name == other.Name && Students == other.Students && ClassNames.SequenceEqual(other.ClassNames);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
            // hash = (hash * 397) ^ Students.GetHashCode();
            // foreach (var c in ClassNames)
            //     hash = (hash * 397) ^ c.GetHashCode();
            // return hash;
        }
    }
}
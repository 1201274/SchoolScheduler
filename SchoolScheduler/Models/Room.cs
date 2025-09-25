namespace SchoolScheduler.Models
{
    public class Room
    {
        public string Name { get; set; }
        public RoomType Type { get; set; }
        public int Capacity { get; set; }
        public Department Department { get; set; }

        public Room(string name, RoomType type, int capacity, Department department)
        {
            Name = name;
            Type = type;
            Capacity = capacity;
            Department = department;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Room other)
            {
                return Name == other.Name &&
                       Department == other.Department;
                // return Name == other.Name &&
                //        Type == other.Type &&
                //        Capacity == other.Capacity &&
                //        Department == other.Department;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = Name?.GetHashCode() ?? 0;
            hash = (hash * 397) ^ Department.GetHashCode();
            return hash;
            // int hash = Name?.GetHashCode() ?? 0;
            // hash = (hash * 397) ^ Type.GetHashCode();
            // hash = (hash * 397) ^ Capacity.GetHashCode();
            // hash = (hash * 397) ^ Department.GetHashCode();
            // return hash;
        }
    }

    [Flags]
    public enum RoomType
    {
        AUDITORIUM = 1 << 0, REGULAR = 1 << 1, LAB = 1 << 2, ONLINE = 1 << 3
    }
}
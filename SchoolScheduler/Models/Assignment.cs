namespace SchoolScheduler.Models
{
    public class Assignment
    {
        public Subject Subject { get; set; }
        public string Teacher { get; set; }
        public Room Room { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public Class Class { get; set; }
        public ClassType ClassType { get; set; }

        public Assignment Clone()
        {
            return new Assignment
            {
                Subject = Subject, 
                Teacher = Teacher, 
                Room = new Room(Room.Name, Room.Type, Room.Capacity, Room.Department),
                TimeSlot = new TimeSlot(TimeSlot.Day, TimeSlot.StartTime, TimeSlot.EndTime),
                Class = new Class { Name = Class.Name, Students = Class.Students, ClassNames = new List<char>(Class.ClassNames) },
                ClassType = new ClassType
                {
                    Type = ClassType.Type,
                    Classes = ClassType.Classes.Select(c => new Class { Name = c.Name, Students = c.Students, ClassNames = new List<char>(c.ClassNames) }).ToList(),
                    TimeSlotsPerWeek = ClassType.TimeSlotsPerWeek,
                    TeacherNames = new List<string>(ClassType.TeacherNames),
                    PreferredRoomType = ClassType.PreferredRoomType
                }
            };
        }
    }
}
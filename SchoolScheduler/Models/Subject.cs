namespace SchoolScheduler.Models
{
    public class Subject
    {
        public string Name { get; set; }
        public int Students { get; set; }
        public List<ClassType> ClassTypes { get; set; }
        public Department Department { get; set; }
        public Course Course { get; set; }

        public Subject() { }

        public Subject(string name, int students, Department department, Course course, Dictionary<ClassTypeEnum, (int timeSlotsPerWeek, List<string> teachers, RoomType preferredRoomType)> typeConfig, int maxStudentsPerGroup = 25)
        {
            Name = name;
            Students = students;
            Department = department;
            Course = course;
            ClassTypes = new List<ClassType>();

            foreach (var kvp in typeConfig)
            {
                var type = kvp.Key;
                var (timeSlotsPerWeek, teacherNames, preferredRoomType) = kvp.Value;

                int maxNumberGroupsPerLesson = type switch
                {
                    ClassTypeEnum.T => 4,
                    ClassTypeEnum.TP => 2,
                    ClassTypeEnum.PL => 1,
                    _ => 1
                };


                var classes = DivideClasses(students, maxStudentsPerGroup, maxNumberGroupsPerLesson)
                    .Select((i) => new Class
                    {
                        Name = $"{Name}_{type}_{i.Name}",
                        Students = i.Students,
                        ClassNames = i.ClassNames
                    }).ToList();

                ClassTypes.Add(new ClassType
                {
                    Type = type,
                    Classes = classes,
                    TimeSlotsPerWeek = timeSlotsPerWeek,
                    TeacherNames = teacherNames,
                    PreferredRoomType = preferredRoomType
                });
            }
        }

        List<Class> DivideClasses(int students, int maxStudentsPerGroup, int maxPerGroup)
        {
            
            int numberOfGroups = (int)Math.Ceiling((double)students / maxStudentsPerGroup);

            var alphabet = Enumerable.Range('A', numberOfGroups).Select(x => (char)x).ToList();
            int totalLetters = alphabet.Count;

            int numGroups = (int)Math.Ceiling((double)totalLetters / maxPerGroup);

            int baseSize = totalLetters / numGroups;
            int extras = totalLetters % numGroups;

            var result = new List<Class>();
            int index = 0;

            for (int i = 0; i < numGroups; i++)
            {
                int groupSize = baseSize + (i < extras ? 1 : 0);
                List<char> letters = alphabet.Skip(index).Take(groupSize).ToList();
                string group = new string(letters.ToArray());
                
                int studentsInThisGroup = (int)Math.Ceiling((double)students / numGroups);
                result.Add(new Class
                {
                    Name = group,
                    Students = studentsInThisGroup,
                    ClassNames = letters
                });
                
                index += groupSize;
            }

            return result;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Subject other)
            {
                return Name == other.Name &&
                    //    Students == other.Students &&
                       Department == other.Department &&
                       Course.Equals(other.Course) &&
                       ClassTypes.SequenceEqual(other.ClassTypes);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = Name?.GetHashCode() ?? 0;
            // hash = (hash * 397) ^ Students.GetHashCode();
            hash = (hash * 397) ^ Department.GetHashCode();
            hash = (hash * 397) ^ (Course?.GetHashCode() ?? 0);
            foreach (var ct in ClassTypes)
                hash = (hash * 397) ^ (ct?.GetHashCode() ?? 0);
            return hash;
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace SchoolScheduler.Models
{
    public class ClassType
    {
        public ClassTypeEnum Type { get; set; }
        public List<Class> Classes { get; set; }
        public int TimeSlotsPerWeek { get; set; }
        public List<string> TeacherNames { get; set; }
        public RoomType PreferredRoomType { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is ClassType other)
            {
                return Type == other.Type &&
                       Classes.SequenceEqual(other.Classes);
                // return Type == other.Type &&
                //        HoursPerWeek == other.HoursPerWeek &&
                //        TeacherNames.SequenceEqual(other.TeacherNames) &&
                //        PreferredRoomType == other.PreferredRoomType &&
                //        Classes.SequenceEqual(other.Classes);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = Type.GetHashCode();
            foreach (var c in Classes)
                hash = (hash * 397) ^ (c?.GetHashCode() ?? 0);
            return hash;
            // int hash = Type.GetHashCode();
            // hash = (hash * 397) ^ HoursPerWeek.GetHashCode();
            // hash = (hash * 397) ^ PreferredRoomType.GetHashCode();
            // foreach (var t in TeacherNames)
            //     hash = (hash * 397) ^ (t?.GetHashCode() ?? 0);
            // foreach (var c in Classes)
            //     hash = (hash * 397) ^ (c?.GetHashCode() ?? 0);
            // return hash;
        }

        public ClassType()
        {
            Classes = new List<Class>();
            TeacherNames = new List<string>();
        }
    }

    public enum ClassTypeEnum
    {
        T = 0,TP = 1,OT = 2,PL = 3,
    }
}

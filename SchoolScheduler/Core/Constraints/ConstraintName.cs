using System.ComponentModel;

namespace SchoolScheduler.Core.Constraints
{
    public enum ConstraintName
    {
        [Description("Multiple Assignments In Same TimeSlot For Classes")]
        [Weight(7)]
        MultipleAssignmentsInSameTimeSlotForClasses,

        [Description("Continuous Class Gaps")]
        [Weight(5)]
        ContinuousClassGaps,
        
        [Description("Continuous Class Different Teachers")]
        [Weight(5)]
        ContinuousClassDifferentTeachers,
        
        [Description("Continuous Class Different Rooms")]
        [Weight(5)]
        ContinuousClassDifferentRooms,
        
        [Description("Student Gaps")]
        [Weight(1)]
        StudentGaps,

        [Description("Incorrect Order Of Classes")]
        [Weight(2)]
        IncorrectOrderOfClasses,

        [Description("Out Of Department Room Usage")]
        [Weight(2)]
        OutOfDepartmentRoomUsage,

        [Description("Preferred Room Type Constraint")]
        [Weight(3)]
        PreferredRoomTypeConstraint,

        [Description("Students Min 3 Hours Day")]
        [Weight(2)]
        StudentsMin3HoursDay,

        [Description("Assignment Load Violation")]
        [Weight(9)]
        AssignmentLoadViolation,

        [Description("Teacher Conflict")]
        [Weight(9)]
        TeacherConflict,

        [Description("Room Conflict")]
        [Weight(9)]
        RoomConflict,

        [Description("Room Capacity Exceeded")]
        [Weight(9)]
        RoomCapacityExceeded,

        [Description("Students Lunch Break")]
        [Weight(7)]
        StudentsLunchBreak,

        [Description("Students Dinner Break")]
        [Weight(7)]
        StudentsDinnerBreak,

        [Description("Teachers Lunch Break")]
        [Weight(6)]
        TeachersLunchBreak,

        [Description("Teachers Dinner Break")]
        [Weight(6)]
        TeachersDinnerBreak,

        [Description("Teachers Max 8 Hours Day")]
        [Weight(8)]
        TeachersMax8HoursDay,

        [Description("Students Max 8 Hours Day")]
        [Weight(8)]
        StudentsMax8HoursDay
    }

    public class WeightAttribute : Attribute
    {
        public int Weight { get; }

        public WeightAttribute(int weight)
        {
            Weight = weight;
        }
    }

    public static class ConstraintNameExtensions
    {
        public static int GetWeight(this ConstraintName constraint)
        {
            var member = typeof(ConstraintName).GetMember(constraint.ToString()).FirstOrDefault();
            var attribute = member?.GetCustomAttributes(typeof(WeightAttribute), false)
                .FirstOrDefault() as WeightAttribute;

            return attribute?.Weight ?? 1; // Default weight is 1 if none specified
        }
    }


}
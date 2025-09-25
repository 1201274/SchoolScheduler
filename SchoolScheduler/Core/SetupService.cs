using SchoolScheduler.Core.Constraints;
using SchoolScheduler.Core.Initializers;
using SchoolScheduler.Models;

namespace SchoolScheduler.Core
{
    public class SetupService
    {
        public List<Subject> Subjects { get; private set; }
        public List<Teacher> Teachers { get; private set; }
        public List<Room> Rooms { get; private set; }
        public List<TimeSlot> TimeSlots { get; private set; }
        public List<Constraint> Constraints { get; private set; }

        public void Initialize()
        {
            Subjects = new SubjectInitializer().Initialize();
            Rooms = new RoomInitializer().Initialize();
            TimeSlots = new TimeSlotInitializer().Initialize();
            Constraints = new ConstraintInitializer().Initialize();
        }
    }
}

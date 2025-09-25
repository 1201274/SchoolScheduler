using SchoolScheduler.Models;

namespace SchoolScheduler.Core.Correction
{
    public static class RoomCorrections
    {
        public static void FixConflicts(List<Assignment> assignments, List<Room> rooms)
        {
            var conflicts = assignments
                .GroupBy(a => new { a.TimeSlot, a.Room.Name })
                .Where(g => g.Count() > 1);

            foreach (var group in conflicts)
            {
                var excess = group.Skip(1).ToList();
                foreach (var assignment in excess)
                    ImproveRoomAssignment(assignment, assignments, rooms, forceChange: true);
            }
        }

        public static void FixCapacity(List<Assignment> assignments, List<Room> rooms)
        {
            var assignmentsWithCapacityIssues = assignments
                .Where(a => a.Class.Students > a.Room.Capacity)
                .ToList();
            foreach (var assignment in assignmentsWithCapacityIssues)
                ImproveRoomAssignment(assignment, assignments, rooms);

        }

        public static void ImproveRoomAssignments(List<Assignment> assignments, List<Room> rooms)
        {
            var assignmentsToImprove = assignments
                .Where(a => !a.ClassType.PreferredRoomType.HasFlag(a.Room.Type) ||
                            a.Room.Department != a.Subject.Department)
                .ToList();

            foreach (var assignment in assignmentsToImprove)
                ImproveRoomAssignment(assignment, assignments, rooms);

        }

        public static void ImproveRoomAssignment(Assignment assignment, List<Assignment> assignments, List<Room> rooms, bool forceChange = false)
        {
            var timeSlot = assignment.TimeSlot;
            var classSize = assignment.Class.Students;
            var requiredType = assignment.ClassType.PreferredRoomType;
            var preferredDept = assignment.Subject.Department;

            var usedRooms = assignments
                .Where(a => a.TimeSlot.Equals(timeSlot))
                .Select(a => a.Room)
                .ToHashSet();

            var availableRooms = rooms
                .Where(r => !usedRooms.Contains(r))
                .ToList();

            // STEP 1: Available + Enough Capacity + RoomType + Dept
            var preferredRooms = availableRooms
                .Where(r => r.Capacity >= classSize &&
                            r.Department == preferredDept &&
                            r.Type.HasFlag(requiredType))
                .OrderBy(r => r.Capacity)
                .ToList();

            if (preferredRooms.Any())
            {
                assignment.Room = preferredRooms.First();
                return;
            }

            // STEP 2: Available + Enough Capacity + (RoomType || Dept)
            var fallbackRooms = availableRooms
                .Where(r => r.Capacity >= classSize &&
                        (r.Department == preferredDept || r.Type.HasFlag(requiredType)))
                .OrderBy(r => r.Capacity)
                .ToList();

            if (fallbackRooms.Any())
            {
                assignment.Room = fallbackRooms.First();
                return;
            }

            // STEP 3: Swap with other assignments - Available + Enough Capacity + RoomType + Dept
            // var sameTimeAssignments = assignments.Where(a => a.TimeSlot.Equals(timeSlot)).ToList();
            var sameTimeAssignments = assignments.Where(a =>
                    a.TimeSlot.Equals(timeSlot) &&
                    a.Class.Students <= assignment.Room.Capacity &&
                    (assignment.Room.Type.HasFlag(a.ClassType.PreferredRoomType) ||
                    a.Room.Department != assignment.Subject.Department))
                .ToList();


            var preferredRoomsToSwap = sameTimeAssignments
                .Where(r => r.Room.Capacity >= classSize &&
                            r.Room.Department == preferredDept &&
                            r.Room.Type.HasFlag(requiredType))
                .OrderBy(r => r.Room.Capacity)
                .ToList();

            if (preferredRoomsToSwap.Any())
            {
                var temp = assignment.Room;
                assignment.Room = preferredRoomsToSwap.First().Room;
                preferredRoomsToSwap.First().Room = temp;
                return;
            }

            // STEP 4: Swap with other assignments - Available + Enough Capacity + (RoomType || Dept)
            var fallbackRoomsToSwap = sameTimeAssignments
                .Where(r => r.Room.Capacity >= classSize &&
                        (r.Room.Department == preferredDept || r.Room.Type.HasFlag(requiredType)))
                .OrderBy(r => r.Room.Capacity)
                .ToList();

            if (fallbackRoomsToSwap.Any())
            {
                var temp = assignment.Room;
                assignment.Room = fallbackRoomsToSwap.First().Room;
                fallbackRoomsToSwap.First().Room = temp;
                return;
            }

            // STEP 5: If capacity not met, try to find a room with increased capacity
            if (assignment.Room.Capacity < classSize)
            {
                var betterOrFallbackRooms = availableRooms
                    .Where(r => r.Capacity > assignment.Room.Capacity)
                    .OrderBy(r => r.Capacity)
                    .Concat(
                        availableRooms
                        .Where(r => r.Capacity < classSize)
                        .OrderByDescending(r => r.Capacity)
                    )
                    .ToList();

                if (betterOrFallbackRooms.Any())
                {
                    assignment.Room = betterOrFallbackRooms.First();
                    return;
                }
            }

            // STEP 6: If forceChange is true, to resolve conflits, assign any available room
            if (forceChange)
            {
                var anyAvailableRoom = availableRooms
                    .OrderByDescending(r => r.Capacity)
                    .FirstOrDefault();

                if (anyAvailableRoom != null)
                {
                    assignment.Room = anyAvailableRoom;
                    return;
                }
            }
        }
    }
}

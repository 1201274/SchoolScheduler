using System.Collections.Generic;
using SchoolScheduler.Models;

namespace SchoolScheduler.Core.Initializers
{
    public class RoomInitializer
    {
        public List<Room> Initialize()
        {
            var rooms = new List<Room>();
            rooms.AddRange(InformaticsRooms());
            return rooms;
        }

        private List<Room> InformaticsRooms()
        {
            return new List<Room>
            {
                new Room("103", RoomType.REGULAR, 50, Department.INFORMATICS),
                new Room("105", RoomType.REGULAR, 50, Department.INFORMATICS),
                new Room("107", RoomType.REGULAR, 50, Department.INFORMATICS),
                new Room("109", RoomType.LAB, 50, Department.INFORMATICS),
                new Room("202", RoomType.AUDITORIUM, 100, Department.INFORMATICS),
                new Room("203", RoomType.AUDITORIUM, 200, Department.INFORMATICS),
                new Room("204", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("205", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("206", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("207", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("208", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("209", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("301", RoomType.AUDITORIUM, 100, Department.INFORMATICS),
                new Room("302", RoomType.AUDITORIUM, 100, Department.INFORMATICS),
                new Room("303", RoomType.AUDITORIUM, 100, Department.INFORMATICS),
                new Room("306", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("309", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("310", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("311", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("401", RoomType.AUDITORIUM, 100, Department.INFORMATICS),
                new Room("402", RoomType.REGULAR, 50, Department.INFORMATICS),
                new Room("403", RoomType.REGULAR, 50, Department.INFORMATICS),
                new Room("404", RoomType.REGULAR, 50, Department.INFORMATICS),
                new Room("405", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("407", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("408", RoomType.LAB, 30, Department.INFORMATICS),
                new Room("409", RoomType.LAB, 30, Department.INFORMATICS),
            };
        }
    }
}

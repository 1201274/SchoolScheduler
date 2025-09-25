namespace SchoolScheduler.Models
{
    public class TimeSlot : IComparable<TimeSlot>
    {
        public Day Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int Time { get; set; } // in minutes
        public TimeOfDay TimeOfDay{ get; set; }

        public TimeSlot(Day day, TimeOnly startTime, TimeOnly endTime)
        {
            Day = day;
            StartTime = startTime;
            EndTime = endTime;
            Time = (int)(endTime - startTime).TotalMinutes;

            if (startTime.Hour < 13) TimeOfDay = TimeOfDay.MORNING;
            else if (startTime.Hour < 18) TimeOfDay = TimeOfDay.AFTERNOON;
            else if (startTime.Hour < 21) TimeOfDay = TimeOfDay.EVENING;
            else TimeOfDay = TimeOfDay.NIGHT;
        }

        public TimeSlot(Day day, TimeOnly startTime, int time)
            : this(day, startTime, startTime.AddMinutes(time))
        {}

        public TimeSlot()
        {}

        public override string ToString() => $"{Enum.GetName(Day)} {StartTime: hh\\:mm} - {EndTime:hh\\:mm}";

        public override bool Equals(object? obj)
        {
            if (obj is TimeSlot other)
            {
                return Day == other.Day && StartTime == other.StartTime && EndTime == other.EndTime && TimeOfDay == other.TimeOfDay;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = Day.GetHashCode();
            hash = (hash * 397) ^ StartTime.GetHashCode();
            hash = (hash * 397) ^ EndTime.GetHashCode();
            hash = (hash * 397) ^ TimeOfDay.GetHashCode();
            return hash;
        }
        
        public int CompareTo(TimeSlot? other)
        {
            if (other == null) return 1;

            int dayComparison = Day.CompareTo(other.Day);
            if (dayComparison != 0) return dayComparison;

            return StartTime.CompareTo(other.StartTime);
        }
    }

    public enum Day
    {
        SUN = 0, MON = 1, TUE = 2, WED = 3, THU = 4, FRI = 5, SAT = 6
    }

    public enum TimeOfDay
    {
        MORNING = 0, AFTERNOON = 1, EVENING = 2, NIGHT = 3
    }
}
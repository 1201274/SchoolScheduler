namespace SchoolScheduler.Core
{
    using System.Collections.Generic;
    using SchoolScheduler.Core.Constraints;

    public class Constants
    {
        public static readonly TimeOnly LunchStartTime = new TimeOnly(12, 0);
        public static readonly TimeOnly LunchEndTime = new TimeOnly(14, 0);
        public static readonly TimeOnly DinnerStartTime = new TimeOnly(20, 0);
        public static readonly TimeOnly DinnerEndTime = new TimeOnly(22, 0);
        public static readonly TimeSpan MinMealBreakDuration = new TimeSpan(0, 30, 0);
    }
}
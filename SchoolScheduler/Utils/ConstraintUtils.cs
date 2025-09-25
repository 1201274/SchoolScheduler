using SchoolScheduler.Models;

namespace SchoolScheduler.Utils
{
    public static class ConstraintUtils
    {
        public static void GetMaxGap(List<Assignment> group, TimeOnly windowTimeStart, TimeOnly windowTimeEnd, out TimeSpan maxGap)
        {
            var gaps = GetGaps(group, windowTimeStart, windowTimeEnd);
            maxGap = gaps.Any() ? gaps.Max(g => g) : windowTimeEnd - windowTimeStart;
        }


        public static List<TimeSpan> GetGaps(List<Assignment> group, TimeOnly? windowTimeStart = null, TimeOnly? windowTimeEnd = null)
        {
            var intervals = group
                .Select(a => (a.TimeSlot.StartTime, a.TimeSlot.EndTime))
                .OrderBy(i => i.StartTime)
                .ToList();

            var merged = new List<(TimeOnly Start, TimeOnly End)>();

            foreach (var interval in intervals)
            {
                if (merged.Count == 0)
                {
                    merged.Add(interval);
                }
                else
                {
                    var last = merged[^1];
                    if (interval.StartTime <= last.End) // Overlapping or adjacent
                    {
                        merged[^1] = (last.Start, TimeOnly.FromTimeSpan(
                            TimeSpan.FromTicks(Math.Max(last.End.ToTimeSpan().Ticks, interval.EndTime.ToTimeSpan().Ticks))
                        ));
                    }
                    else
                    {
                        merged.Add(interval);
                    }
                }
            }

            var startBound = windowTimeStart ?? intervals.First().StartTime;
            var endBound = windowTimeEnd ?? intervals.Last().EndTime;
            var gaps = new List<TimeSpan>();
            var current = startBound;

            foreach (var (start, end) in merged)
            {
                if (start > current)
                {
                    var gapStart = current > startBound ? current : startBound;
                    var gapEnd = start < endBound ? start : endBound;

                    if (gapEnd > gapStart)
                        gaps.Add(gapEnd - gapStart);
                }

                current = current > end ? current : end;
            }

            if (current < endBound)
            {
                gaps.Add(endBound - current);
            }

            return gaps;
        }


    }
}